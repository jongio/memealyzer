#!/bin/bash
set -o allexport; source ../../../../.env; set +o allexport
cd ../../../../src/net/Services/Functions
func azure functionapp publish $AZURE_FUNCTION_APP_NAME --force
#az functionapp deployment source config-zip -g $AZURE_RESOURCE_GROUP $AZURE_FUNCTION_APP_NAME
