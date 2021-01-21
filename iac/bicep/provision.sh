#!/bin/bash 
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

./gen.sh

az deployment sub create --debug -n ${BASENAME} -l ${REGION} -f main.json -p basename=${BASENAME} location=${REGION} failover_location=${FAILOVER_REGION} cli_user_id=${CLI_USER_ID}