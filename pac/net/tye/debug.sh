#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

export ENV=local

source ./genfiles.sh

source $ROOT/scripts/login.sh

echo "DEBUGGING APPLICATION"

tye run --logs console -v debug --watch --tags $ENV --debug memealyzernetapi
