#!/bin/bash 
set -euo pipefail

if [ $# -eq 0 ]
  then
    echo "You need to run this script with the basename you used to created your resources. i.e., ./perms.sh memealyzer01"
    exit
fi

BASENAME=$1
SCOPE_RG=$(az group show -g ${BASENAME}rg --query id -o tsv)
SCOPE_AKS=$(az group show -g ${BASENAME}aksnodes --query id -o tsv)

function roles {
  az role assignment create --assignee-object-id $1 --role "Storage Blob Data Contributor" --assignee-principal-type $2 --scope $3
  az role assignment create --assignee-object-id $1 --role "Storage Queue Data Contributor" --assignee-principal-type $2 --scope $3
  az role assignment create --assignee-object-id $1 --role "Storage Queue Data Message Processor" --assignee-principal-type $2 --scope $3
  az role assignment create --assignee-object-id $1 --role "Azure Service Bus Data Receiver" --assignee-principal-type $2 --scope $3
  az role assignment create --assignee-object-id $1 --role "Azure Service Bus Data Sender" --assignee-principal-type $2 --scope $3
  az role assignment create --assignee-object-id $1 --role "Cognitive Services User" --assignee-principal-type $2 --scope $3
  az role assignment create --assignee-object-id $1 --role "App Configuration Data Owner" --assignee-principal-type $2 --scope $3
  az role assignment create --assignee-object-id $1 --role "AcrPush" --assignee-principal-type $2 --scope $3
  az role assignment create --assignee-object-id $1 --role "AcrPull" --assignee-principal-type $2 --scope $3
}

function assign {
  roles $1 $2 $SCOPE_RG
  roles $1 $2 $SCOPE_AKS

  echo "Setting KeyVault Policy for Azure CLI User" 
  az keyvault set-policy -g "${BASENAME}"rg -n "${BASENAME}"kv --object-id $1 --secret-permissions get set list delete
}

echo "Assigning Roles to Currently Logged in Azure CLI User"
assign ${CLI_USER_ID} User

echo "Assigning Roles to AKS Cluster Managed Identity"
assign $(az aks show -g ${BASENAME}rg -n ${BASENAME}aks --query identity.principalId -o tsv) ServicePrincipal

echo "Assigning Roles to AKS Nodes Managed Identity"
assign $(az identity show -g ${BASENAME}aksnodes -n ${BASENAME}aks-agentpool --query principalId -o tsv) ServicePrincipal

echo "Assigning Roles to Function App Managed Identity"
assign $(az functionapp identity show -g ${BASENAME}rg -n ${BASENAME}function --query principalId -o tsv) ServicePrincipal

echo "Done"