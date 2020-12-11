#!/bin/bash 
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

./gen.sh

az deployment sub what-if -n ${BASENAME}rg -l ${LOCATION} -f rg.json -p basename=${BASENAME} location=${LOCATION} failover_location=${FAILOVER_REGION} cli_user_id=${CLI_USER_ID}