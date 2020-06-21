variable "script" {
    description = "Script to run"
    type        = string
 }
 
 variable "environment" {
  type        = map(string)
  default     = {}
  description = "(Optional) Map of environment variables to pass to the command"
}