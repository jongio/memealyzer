#!/bin/bash 
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

./gen.sh

source $ROOT/scripts/login.sh

az deployment sub create --debug -n ${BASENAME} -l ${REGION} -f main.json -p basename=${BASENAME} location=${REGION} failoverLocation=${FAILOVER_REGION} principalId=${CLI_USER_ID}