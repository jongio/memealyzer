#!/bin/bash 
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

source $ROOT/scripts/login.sh

az deployment group create --debug -n ${BASENAME}rgpolicy -g ${BASENAME}rg -f policy.bicep -p basename=${BASENAME} principalId=${CLI_USER_ID}