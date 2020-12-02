#!/bin/bash 
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

./gen.sh

az deployment sub create --debug -n ${BASENAME}rg -l ${REGION} -f rg.json -p basename=${BASENAME} location=${REGION} failover_location=${FAILOVER_REGION} cli_user_id=${CLI_USER_ID}
#az deployment group create --debug -n ${BASENAME}resources -g ${BASENAME}rg -f resources.json -p basename=${BASENAME} location=${REGION} failover_location=${FAILOVER_REGION} cli_user_id=${CLI_USER_ID}
#az deployment group create --debug -n ${BASENAME}aksperms -g ${BASENAME}aksnodes -f permsaks.json -p basename=${BASENAME}

# Create Service Bus Queues
#az servicebus queue create -n messages -g ${BASENAME}rg --namespace-name ${BASENAME}sb --default-message-time-to-live PT30S
#az servicebus queue create -n sync -g ${BASENAME}rg --namespace-name ${BASENAME}sb --default-message-time-to-live PT30S

# App Configuration Default Values
#az appconfig kv set -y -n ${BASENAME}appconfig --key borderStyle --value solid

# Azure Function MI Permissions to KV
# We need this becuase of the cyclic dependency on KV and AF
FUNCTION_MI_ID=$(az functionapp identity show -n memealyzerdevfunction -g memealyzerdevrg --query principalId -o tsv)
az keyvault set-policy -g ${BASENAME}rg -n ${BASENAME}kv --object-id $FUNCTION_MI_ID --secret-permissions get set list delete

# Apply all the necessary permissions that we couldn't get to work with bicep
source ../scripts/perms.sh ${BASENAME}