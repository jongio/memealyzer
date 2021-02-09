#!/bin/bash 
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

./gen.sh

source $ROOT/scripts/login.sh

az deployment sub what-if -n ${BASENAME}rg -l ${LOCATION} -f rg.json -p basename=${BASENAME} location=${LOCATION} failoverLocation=${FAILOVER_REGION} principalId=${CLI_USER_ID}