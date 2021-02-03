#!/bin/bash
set -euo pipefail

# Custom Domain for Cognitive Services
az cognitiveservices account update --name $FORM_RECOGNIZER_NAME -g $RESOURCE_GROUP_NAME --custom-domain $FORM_RECOGNIZER_NAME
az cognitiveservices account update --name $TEXT_ANALYTICS_NAME -g $RESOURCE_GROUP_NAME --custom-domain $TEXT_ANALYTICS_NAME


# Managed Identity for Cognitive Services
az cognitiveservices account identity assign -n $FORM_RECOGNIZER_NAME -g $RESOURCE_GROUP_NAME
az cognitiveservices account identity assign -n $TEXT_ANALYTICS_NAME -g $RESOURCE_GROUP_NAME

# Managed Identity for App Configuration
az appconfig identity assign -n $APP_CONFIG_NAME -g $RESOURCE_GROUP_NAME

# App Configuration Default Values
az appconfig kv set -y -n $APP_CONFIG_NAME --key borderStyle --value solid

# Azure Function MI Permissions to KV
# We need this becuase of the cyclic dependency on KV and AF
az keyvault set-policy -g $RESOURCE_GROUP_NAME -n $KEY_VAULT_NAME --object-id $FUNCTION_MI_ID --secret-permissions get set list delete
