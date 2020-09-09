#!/bin/bash
echo "Deploying Azure Function"
cd ../../../../src/net/Services/Functions
func azure functionapp publish $AZURE_FUNCTION_APP_NAME --force
