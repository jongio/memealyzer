#!/bin/bash 
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

source $ROOT/scripts/login.sh

echo "Deleting resource group: ${BASENAME}rg"
az group delete -n ${BASENAME}rg
az deployment sub delete --debug -n ${BASENAME}
az deployment sub wait --debug --deleted -n ${BASENAME}