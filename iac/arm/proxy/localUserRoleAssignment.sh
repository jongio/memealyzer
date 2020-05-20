#!/bin/bash

echo "Assigning Storage Blob Data Contributor Role to Local User"
principalId=$(az ad signed-in-user show --query 'objectId' -o tsv)
az role assignment create --assignee-object-id $principalId --role "Storage Blob Data Contributor"
