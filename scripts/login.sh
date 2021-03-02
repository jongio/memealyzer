#!/bin/bash
set -euo pipefail

echo "AZURE LOGIN"

EXPIRED_TOKEN=$(az ad signed-in-user show --query 'objectId' -o tsv || true)

if [[ -z "$EXPIRED_TOKEN" ]]; then
    az login -o none
fi

echo "AZURE SUBSCRIPTION"

if [[ -z "${AZURE_SUBSCRIPTION_ID:-}" ]]; then
    ACCOUNT=$(az account show --query '[id,name]')

    echo "You can set the \`AZURE_SUBSCRIPTION_ID\` environment variable in the \`.env\` file if you don't want to be prompted every time you run these scripts."
    echo $ACCOUNT
    
    read -r -p "Do you want to use the above subscription? (y/n) " response
    case "$response" in
        [yY][eE][sS]|[yY]) 
            ;;
        *)
            echo "Use the \`az account set\` command to set the subscription you'd like to use and re-run this script."
            exit 0
            ;;
    esac
else
    echo "Found AZURE_SUBSCRIPTION_ID environment variable. Setting active subscription to: $AZURE_SUBSCRIPTION_ID"
    az account set -s $AZURE_SUBSCRIPTION_ID
fi

CLI_USER_ID=$(az ad signed-in-user show --query 'objectId' -o tsv)