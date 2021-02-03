#!/bin/bash 
set -euo pipefail

terraform workspace select $BASENAME || terraform workspace new $BASENAME
terraform workspace select $BASENAME
