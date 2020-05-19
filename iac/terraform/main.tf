resource "azurerm_resource_group" "rg" {
  name     = var.basename
  location = var.location
}

resource "azurerm_storage_account" "storage" {
  name                     = "${var.basename}storage"
  location                 = var.location
  resource_group_name      = azurerm_resource_group.rg.name
  account_tier             = "Standard"
  account_replication_type = "LRS"
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
    "AZUREBLOBSTORAGE:SERVICEURI"    = azurerm_storage_account.storage.primary_blob_endpoint,
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.logging.instrumentation_key
  }

  logs {
    http_logs {
      file_system {
        retention_in_days = 1
        retention_in_mb   = 25
      }
    }
  }

  depends_on = [azurerm_application_insights.logging]
}

resource "azurerm_application_insights" "logging" {
  name                = "${var.basename}ai"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  application_type    = "web"
}

resource "null_resource" "azure-cli" {
  triggers = {
    default_trigger = timestamp()
  }
  provisioner "local-exec" {
    command = "az webapp log config --application-logging true --level information -n ${azurerm_app_service.app.name} -g ${azurerm_resource_group.rg.name} -o none"
  }

  depends_on = [azurerm_app_service.app]
}

resource "azurerm_role_assignment" "app_mi_storage" {
  scope                = azurerm_storage_account.storage.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_app_service.app.identity[0].principal_id
}

resource "azurerm_role_assignment" "user_cli_storage" {
  scope                = azurerm_storage_account.storage.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = data.azurerm_client_config.current.object_id
}

output "storage_uri" {
  value = azurerm_storage_account.storage.primary_blob_endpoint
}

output "ai_key" {
  value = azurerm_application_insights.logging.instrumentation_key
}
