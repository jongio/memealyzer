#Set the terraform required version
terraform {
  required_version = ">= 0.12.6"
}

# Configure the Azure Provider
provider "azurerm" {
  # It is recommended to pin to a given version of the Provider
  version = "~>2"
  features {}
}

provider "random" {
  version = "~>2"
}

provider "null" {
  version = "~> 2.1"
}

# Data

# Make client_id, tenant_id, subscription_id and object_id variables
data "azurerm_client_config" "current" {}
