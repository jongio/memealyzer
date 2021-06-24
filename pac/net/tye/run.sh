#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

export ENV=local

source ./genfiles.sh

source $ROOT/scripts/login.sh

echo "RUNNING APPLICATION"

source $ROOT/scripts/cosmos.sh

tye run --watch --logs console -v debug --tags $ENV