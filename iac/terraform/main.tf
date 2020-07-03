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
      "get", "set", "list"
    ]

  }
}

resource "azurerm_key_vault_secret" "key_vault_secret" {
  name         = "cosmoskey"
  value        = azurerm_cosmosdb_account.cosmos_account.primary_master_key
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

resource "azurerm_app_service_plan" "plan" {
  name                = "${var.basename}plan"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  sku {
    tier = "Basic"
    size = "B1"
  }
}

resource "azurerm_app_service" "app" {
  name                = "${var.basename}app"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  app_service_plan_id = azurerm_app_service_plan.plan.id


  identity {
    type = "SystemAssigned"
  }

  app_settings = {
    "AZURE_STORAGE_BLOB_ENDPOINT"       = azurerm_storage_account.storage.primary_blob_endpoint,
    "AZURE_STORAGE_BLOB_CONTAINER_NAME" = "blobs",
    "AZURE_STORAGE_QUEUE_ENDPOINT"      = azurerm_storage_account.storage.primary_queue_endpoint,
    "AZURE_STORAGE_QUEUE_NAME"          = "messages",
    "AZURE_STORAGE_QUEUE_MSG_COUNT"     = "10",
    "AZURE_TEXT_ANALYTICS_ENDPOINT"     = azurerm_cognitive_account.text_analytics.endpoint,
    "AZURE_FORM_RECOGNIZER_ENDPOINT"    = azurerm_cognitive_account.text_analytics.endpoint,
    "AZURE_COSMOS_ENDPOINT"             = azurerm_cosmosdb_account.cosmos_account.endpoint,
    "AZURE_COSMOS_KEY"                  = azurerm_cosmosdb_account.cosmos_account.primary_master_key,
    "AZURE_COSMOS_DB"                   = "azimageai",
    "AZURE_COSMOS_CONTAINER"            = "images",
    "APPINSIGHTS_INSTRUMENTATIONKEY"    = azurerm_application_insights.logging.instrumentation_key,
    "MEME_ENDPOINT"                     = "https://meme-api.herokuapp.com/gimme/wholesomememes",
    "API_ENDPOINT"                      = "http://${var.basename}app.azurewebsites.net:2080"
  }

  logs {
    http_logs {
      file_system {
        retention_in_days = 1
        retention_in_mb   = 25
      }
    }
  }
}

resource "azurerm_app_service" "api" {
  name                = "${var.basename}api"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name
  app_service_plan_id = azurerm_app_service_plan.plan.id


  identity {
    type = "SystemAssigned"
  }

  app_settings = {
    "AZURE_STORAGE_BLOB_ENDPOINT"       = azurerm_storage_account.storage.primary_blob_endpoint,
    "AZURE_STORAGE_BLOB_CONTAINER_NAME" = "blobs",
    "AZURE_STORAGE_QUEUE_ENDPOINT"      = azurerm_storage_account.storage.primary_queue_endpoint,
    "AZURE_STORAGE_QUEUE_NAME"          = "messages",
    "AZURE_STORAGE_QUEUE_MSG_COUNT"     = "10",
    "AZURE_TEXT_ANALYTICS_ENDPOINT"     = azurerm_cognitive_account.text_analytics.endpoint,
    "AZURE_FORM_RECOGNIZER_ENDPOINT"    = azurerm_cognitive_account.text_analytics.endpoint,
    "AZURE_COSMOS_ENDPOINT"             = azurerm_cosmosdb_account.cosmos_account.endpoint,
    "AZURE_COSMOS_KEY"                  = azurerm_cosmosdb_account.cosmos_account.primary_master_key,
    "AZURE_COSMOS_DB"                   = "azimageai",
    "AZURE_COSMOS_CONTAINER"            = "images",
    "APPINSIGHTS_INSTRUMENTATIONKEY"    = azurerm_application_insights.logging.instrumentation_key,
    "MEME_ENDPOINT"                     = "https://meme-api.herokuapp.com/gimme/wholesomememes"
  }

  logs {
    http_logs {
      file_system {
        retention_in_days = 1
        retention_in_mb   = 25
      }
    }
  }
}

resource "azurerm_application_insights" "logging" {
  name                = "${var.basename}ai"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  application_type    = "web"
}

module "script" {
  source = "./modules/script"
  script = "./tweaks.sh"
  environment = {
    APP_NAME             = azurerm_app_service.app.name,
    TEXT_ANALYTICS_NAME  = azurerm_cognitive_account.text_analytics.name,
    FORM_RECOGNIZER_NAME = azurerm_cognitive_account.form_recognizer.name,
    RESOURCE_GROUP_NAME  = azurerm_resource_group.rg.name
  }
}

resource "azurerm_role_assignment" "app_mi_blob_storage" {
  scope                            = azurerm_storage_account.storage.id
  role_definition_name             = "Storage Blob Data Contributor"
  principal_id                     = azurerm_app_service.app.identity[0].principal_id
  skip_service_principal_aad_check = true
}

resource "azurerm_role_assignment" "user_cli_blob_storage" {
  scope                = azurerm_storage_account.storage.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = data.azurerm_client_config.current.object_id
}

resource "azurerm_role_assignment" "app_mi_queue_storage" {
  scope                            = azurerm_storage_account.storage.id
  role_definition_name             = "Storage Queue Data Contributor"
  principal_id                     = azurerm_app_service.app.identity[0].principal_id
  skip_service_principal_aad_check = true
}

resource "azurerm_role_assignment" "user_cli_queue_storage" {
  scope                = azurerm_storage_account.storage.id
  role_definition_name = "Storage Queue Data Contributor"
  principal_id         = data.azurerm_client_config.current.object_id
}

resource "azurerm_role_assignment" "app_mi_queue_msg_storage" {
  scope                            = azurerm_storage_account.storage.id
  role_definition_name             = "Storage Queue Data Message Processor"
  principal_id                     = azurerm_app_service.app.identity[0].principal_id
  skip_service_principal_aad_check = true
}

resource "azurerm_role_assignment" "user_cli_queue_msg_storage" {
  scope                = azurerm_storage_account.storage.id
  role_definition_name = "Storage Queue Data Message Processor"
  principal_id         = data.azurerm_client_config.current.object_id
}

resource "azurerm_role_assignment" "app_mi_cogserv" {
  scope                            = azurerm_storage_account.storage.id
  role_definition_name             = "Cognitive Services User"
  principal_id                     = azurerm_app_service.app.identity[0].principal_id
  skip_service_principal_aad_check = true
}

resource "azurerm_role_assignment" "user_cli_cogserv" {
  scope                = azurerm_storage_account.storage.id
  role_definition_name = "Cognitive Services User"
  principal_id         = data.azurerm_client_config.current.object_id
}

# KEY VAULT ACCESS POLICIES

resource "azurerm_key_vault_access_policy" "user_cli_keyvault" {
  key_vault_id = azurerm_key_vault.key_vault.id

  tenant_id = data.azurerm_client_config.current.tenant_id
  object_id = data.azurerm_client_config.current.object_id

  secret_permissions = [
    "get"
  ]
}

output "AZURE_STORAGE_BLOB_ENDPOINT" {
  value = azurerm_storage_account.storage.primary_blob_endpoint
}

output "AZURE_STORAGE_QUEUE_ENDPOINT" {
  value = azurerm_storage_account.storage.primary_queue_endpoint
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