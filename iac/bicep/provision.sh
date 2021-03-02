#!/bin/bash 
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

source $ROOT/scripts/login.sh

az deployment sub create --debug -n ${BASENAME} -l ${REGION} -f main.bicep -p basename=${BASENAME} location=${REGION} failoverLocation=${FAILOVER_REGION} principalId=${CLI_USER_ID}