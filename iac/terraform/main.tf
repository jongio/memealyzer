resource "azurerm_resource_group" "rg" {
  name     = "${var.basename}rg"
  location = var.location
}

resource "azurerm_storage_account" "storage" {
  name                     = "${var.basename}storage"
  location                 = var.location
  resource_group_name      = azurerm_resource_group.rg.name
  account_tier             = "Standard"
  account_replication_type = "LRS"
  allow_blob_public_access = true
}

resource "azurerm_cognitive_account" "form_recognizer" {
  name                = "${var.basename}formrecognizer"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  kind                = "FormRecognizer"

  sku_name = "S0"
}

resource "azurerm_cognitive_account" "text_analytics" {
  name                = "${var.basename}textanalytics"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  kind                = "TextAnalytics"

  sku_name = "S0"
}

resource "azurerm_key_vault" "key_vault" {
  name                = "${var.basename}keyvault"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = [
      "get", "set", "list", "delete"
    ]
  }

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = azurerm_kubernetes_cluster.aks.kubelet_identity.0.object_id

    secret_permissions = [
      "get", "set", "list", "delete"
    ]
  }
}

resource "azurerm_key_vault_secret" "cosmos_key_secret" {
  name         = "cosmoskey"
  value        = azurerm_cosmosdb_account.cosmos_account.primary_master_key
  key_vault_id = azurerm_key_vault.key_vault.id
}

resource "azurerm_key_vault_secret" "storage_key_secret" {
  name         = "storagekey"
  value        = azurerm_storage_account.storage.primary_access_key
  key_vault_id = azurerm_key_vault.key_vault.id
}

resource "azurerm_key_vault_secret" "signalr_connection_string_secret" {
  name         = "signalrconnectionstring"
  value        = azurerm_signalr_service.signalr.primary_connection_string
  key_vault_id = azurerm_key_vault.key_vault.id
}

resource "azurerm_key_vault_secret" "storage_connection_string_secret" {
  name         = "storageconnectionstring"
  value        = azurerm_storage_account.storage.primary_connection_string
  key_vault_id = azurerm_key_vault.key_vault.id
}

resource "azurerm_cosmosdb_account" "cosmos_account" {
  name                = "${var.basename}cosmosaccount"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"

  enable_automatic_failover = false

  consistency_policy {
    consistency_level = "Session"
  }

  geo_location {
    location          = var.failover_location
    failover_priority = 1
  }

  geo_location {
    prefix            = "${var.basename}cosmosaccountgeo"
    location          = azurerm_resource_group.rg.location
    failover_priority = 0
  }
}

resource "azurerm_cosmosdb_sql_database" "cosmos_sqldb" {
  name                = "azimageai"
  resource_group_name = azurerm_resource_group.rg.name
  account_name        = azurerm_cosmosdb_account.cosmos_account.name
  throughput          = 400
}

resource "azurerm_cosmosdb_sql_container" "cosmos_sqldb_container" {
  name                = "images"
  resource_group_name = azurerm_resource_group.rg.name
  account_name        = azurerm_cosmosdb_account.cosmos_account.name
  database_name       = azurerm_cosmosdb_sql_database.cosmos_sqldb.name
  partition_key_path  = "/uid"
  throughput          = 400

  unique_key {
    paths = ["/uid"]
  }
}


resource "azurerm_kubernetes_cluster" "aks" {
  name                = "${var.basename}aks"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  dns_prefix          = "${var.basename}aks"
  kubernetes_version  = "1.17.7"
  node_resource_group = "${var.basename}aksnodes"

  default_node_pool {
    name       = "default"
    node_count = 1
    vm_size    = "Standard_A2_v2"
  }

  identity {
    type = "SystemAssigned"
  }
}

resource "azurerm_application_insights" "logging" {
  name                = "${var.basename}ai"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  application_type    = "web"
}

# APP CONFIGURATION

resource "azurerm_app_configuration" "appconfig" {
  name                = "${var.basename}appconfig"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "standard"
}

# SIGNALR SERVICE

resource "azurerm_signalr_service" "signalr" {
  name                = "${var.basename}signalr"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name

  sku {
    name     = "Standard_S1"
    capacity = 1
  }

  cors {
    allowed_origins = ["*"]
  }

  features {
    flag  = "ServiceMode"
    value = "Serverless"
  }
}

# Azure CLI Script to fill in Terraform gaps.

module "script" {
  source = "./modules/script"
  script = "./tweaks.sh"
  environment = {
    TEXT_ANALYTICS_NAME  = azurerm_cognitive_account.text_analytics.name,
    FORM_RECOGNIZER_NAME = azurerm_cognitive_account.form_recognizer.name,
    RESOURCE_GROUP_NAME  = azurerm_resource_group.rg.name,
    APP_CONFIG_NAME  = azurerm_app_configuration.appconfig.name
  }
}

# BLOB STORAGE ROLES
resource "azurerm_role_assignment" "mi_blob_storage" {
  scope                            = azurerm_storage_account.storage.id
  role_definition_name             = "Storage Blob Data Contributor"
  principal_id                     = azurerm_kubernetes_cluster.aks.kubelet_identity.0.object_id
  skip_service_principal_aad_check = true
}

# QUEUE STORAGE ROLES
resource "azurerm_role_assignment" "mi_queue_storage" {
  scope                            = azurerm_storage_account.storage.id
  role_definition_name             = "Storage Queue Data Contributor"
  principal_id                     = azurerm_kubernetes_cluster.aks.kubelet_identity.0.object_id
  skip_service_principal_aad_check = true
}

# QUEUE MSG STORAGE ROLES

resource "azurerm_role_assignment" "mi_queue_msg_storage" {
  scope                            = azurerm_storage_account.storage.id
  role_definition_name             = "Storage Queue Data Message Processor"
  principal_id                     = azurerm_kubernetes_cluster.aks.kubelet_identity.0.object_id
  skip_service_principal_aad_check = true
}

# COG SERV ROLES

resource "azurerm_role_assignment" "mi_cogserv" {
  scope                            = data.azurerm_subscription.sub.id
  role_definition_name             = "Cognitive Services User"
  principal_id                     = azurerm_kubernetes_cluster.aks.kubelet_identity.0.object_id
  skip_service_principal_aad_check = true
}

# APP CONFIG ROLES

resource "azurerm_role_assignment" "mi_appconfig" {
  scope                            = data.azurerm_subscription.sub.id
  role_definition_name             = "App Configuration Data Owner"
  principal_id                     = azurerm_kubernetes_cluster.aks.kubelet_identity.0.object_id
  skip_service_principal_aad_check = true
}

output "AZURE_STORAGE_BLOB_ENDPOINT" {
  value = azurerm_storage_account.storage.primary_blob_endpoint
}

output "AZURE_STORAGE_QUEUE_ENDPOINT" {
  value = azurerm_storage_account.storage.primary_queue_endpoint
}

output "AZURE_STORAGE_TABLE_ENDPOINT" {
  value = azurerm_storage_account.storage.primary_table_endpoint
}

output "AZURE_STORAGE_ACCOUNT_NAME" {
  value = azurerm_storage_account.storage.name
}

output "AZURE_FORM_RECOGNIZER_ENDPOINT" {
  value = "https://${azurerm_cognitive_account.form_recognizer.name}.cognitiveservices.azure.com/"
}

output "AZURE_TEXT_ANALYTICS_ENDPOINT" {
  value = "https://${azurerm_cognitive_account.text_analytics.name}.cognitiveservices.azure.com/"
}

output "AZURE_KEYVAULT_ENDPOINT" {
  value = azurerm_key_vault.key_vault.vault_uri
}

output "AZURE_COSMOS_ENDPOINT" {
  value = azurerm_cosmosdb_account.cosmos_account.endpoint
}

output "AKS_IP_ADDRESS" {
  value = "az network public-ip list -g ${azurerm_kubernetes_cluster.aks.node_resource_group} --query '[0].ipAddress' --output tsv"
}

output "AKS_CREDENTIALS" {
  value = "az aks get-credentials --resource-group ${azurerm_resource_group.rg.name} --name ${azurerm_kubernetes_cluster.aks.name}"
}

output "AZURE_APP_CONFIG_ENDPOINT" {
  value = azurerm_app_configuration.appconfig.endpoint
}

output "AZURE_SIGNALR_HOSTNAME" {
  value = azurerm_signalr_service.signalr.hostname
}
