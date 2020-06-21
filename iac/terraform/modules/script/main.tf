provider "null" {
  version = "~> 2.1"
}

resource "null_resource" "script" {
    triggers = {
        default_trigger = timestamp()
    }

    provisioner "local-exec" {
        command = var.script
        environment = var.environment
        interpreter = ["bash"]
    }
}