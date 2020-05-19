variable "basename" {
  type        = string
  description = "The base name for all resources"
}

variable "location" {
  type        = string
  description = "Azure region where to create resources."
  default     = "West US"
}