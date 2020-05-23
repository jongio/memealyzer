#!/bin/bash

baseName=azsdkdemo11
location=westus
assetsBaseUrl=http://ce868b4a.ngrok.io

az deployment sub create -l $location -u "$assetsBaseUrl/azuredeploy.json" --parameters baseName=$baseName

echo "Assigning Storage Blob Data Contributor Role to Local User"

principalId=$(az ad signed-in-user show --query 'objectId' -o tsv)
az role assignment create --assignee-object-id $principalId --role "Storage Blob Data Contributor"
az role assignment create --assignee-object-id $principalId --role "Storage Queue Data Contributor"
az role assignment create --assignee-object-id $principalId --role "Storage Queue Data Message Processor"