#!/bin/bash

echo "Assigning Roles to Currently Logged in Azure CLI User"

principalId=$(az ad signed-in-user show --query 'objectId' -o tsv)

az role assignment create --assignee-object-id $principalId --role "Storage Blob Data Contributor"
az role assignment create --assignee-object-id $principalId --role "Storage Queue Data Contributor"
az role assignment create --assignee-object-id $principalId --role "Storage Queue Data Message Processor"
az role assignment create --assignee-object-id $principalId --role "Cognitive Services User"

echo "Done"