# Create Azure AD App
resource "azuread_application" "app" {
  name                       = "${var.basename}sp"
  available_to_other_tenants = false
}

# Create Service Principal associated with the Azure AD App
resource "azuread_service_principal" "sp" {
  application_id = azuread_application.app.application_id
}

# Generate random string to be used for Service Principal password
resource "random_string" "password" {
  length  = 32
  special = true
}

# Create Service Principal password
resource "azuread_service_principal_password" "sppass" {
  service_principal_id = azuread_service_principal.sp.id
  value                = random_string.password.result
  end_date_relative    = "17520h" #expire in 2 years
}

# Create role assignment for service principal
resource "azurerm_role_assignment" "contributor" {
  scope                = data.azurerm_subscription.sub.id
  role_definition_name = "Contributor"
  principal_id         = azuread_service_principal.sp.id
}

output "AZURE_CLIENT_ID" {
  value = azuread_application.app.application_id
}

output "AZURE_CLIENT_SECRET" {
  value = azuread_service_principal_password.sppass.value
}

output "AZURE_TENANT_ID" {
  value = data.azurerm_client_config.current.tenant_id
}