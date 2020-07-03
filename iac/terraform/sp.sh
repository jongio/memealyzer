#!/bin/bash
# run sudo apt-get install jq

baseName="$1"

echo "Creating Service Principal..."
sp=$(az ad sp create-for-rbac --skip-assignment)
echo $sp
clientId=$(echo $sp | jq -r '.appId')
objectId=$(az ad sp show --id $clientId --query objectId -o tsv)
clientSecret=$(echo $sp | jq -r '.password')
tenantId=$(echo $sp | jq -r '.tenant')

echo "Assigning Roles..."
az role assignment create --assignee-object-id $objectId --assignee-principal-type ServicePrincipal --role "Storage Blob Data Contributor"
az role assignment create --assignee-object-id $objectId --assignee-principal-type ServicePrincipal --role "Storage Queue Data Contributor"
az role assignment create --assignee-object-id $objectId --assignee-principal-type ServicePrincipal --role "Storage Queue Data Message Processor"
az role assignment create --assignee-object-id $objectId --assignee-principal-type ServicePrincipal --role "Cognitive Services User"

echo "Creating Key Vault Access Policies..."
keyVaultName="${baseName}keyvault"
az keyvault set-policy --name $keyVaultName --object-id $objectId --secret-permissions get


echo AZURE_CLIENT_ID=${clientId}
echo AZURE_CLIENT_SECRET=${clientSecret}
echo AZURE_TENANT_ID=${tenantId}
