#!/bin/bash
set -euo pipefail

CLI_USER_ID=$(az ad signed-in-user show --query 'objectId' -o tsv)

source $ROOT/scripts/parseargs.sh
source $ROOT/scripts/loadenv.sh