#!/bin/bash 
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

terraform workspace select $WORKSPACE || terraform workspace new $WORKSPACE
terraform workspace select $WORKSPACE

terraform destroy