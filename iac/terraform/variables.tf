variable "basename" {
  type        = string
  description = "The base name for all resources"
  default     = "azsdkdemo99"
}

variable "location" {
  type        = string
  description = "Azure region where to create resources."
  default     = "West US"
}

variable "failover_location" {
  type        = string
  description = "Cosmos failover"
  default     = "East US"
}