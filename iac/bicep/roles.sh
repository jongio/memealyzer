#!/bin/bash 
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

source $ROOT/scripts/login.sh

az deployment group create --debug -n ${BASENAME}_ROLES_${CLI_USER_ID} -g ${BASENAME}rg -f rolesapp.bicep -p principalId=${CLI_USER_ID} principalType=User resourceGroupName=${BASENAME}rg