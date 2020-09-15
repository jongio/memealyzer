variable "basename" {
  type        = string
  description = "The base name for all resources"
  default     = "memealyzer100"
}

variable "location" {
  type        = string
  description = "Azure region where to create resources."
  default     = "westus2"
}

variable "failover_location" {
  type        = string
  description = "Cosmos failover"
  default     = "eastus2"
}