#!/bin/bash 
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

bicep build roles.bicep

source $ROOT/scripts/login.sh

az deployment group create --debug -n ${BASENAME}_ROLES_${CLI_USER_ID} -g ${BASENAME}rg -f roles.json -p principalId=${CLI_USER_ID} principalType=User resourceGroupName=${BASENAME}rg