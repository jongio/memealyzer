#!/bin/bash
set -euo pipefail

echo "DEPLOYING AZURE FUNCTION"
cd ${ROOT}/src/net/Services/Functions
func azure functionapp publish $AZURE_FUNCTION_APP_NAME --force
