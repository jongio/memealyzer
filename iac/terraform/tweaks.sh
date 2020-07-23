#!/bin/bash
az cognitiveservices account update --name $FORM_RECOGNIZER_NAME -g $RESOURCE_GROUP_NAME --custom-domain $FORM_RECOGNIZER_NAME
az cognitiveservices account update --name $TEXT_ANALYTICS_NAME -g $RESOURCE_GROUP_NAME --custom-domain $TEXT_ANALYTICS_NAME

az resource update -n $FORM_RECOGNIZER_NAME -g $RESOURCE_GROUP_NAME --resource-type "Microsoft.CognitiveServices/accounts" --set identity="{\"type\": \"SystemAssigned\"}"
az resource update -n $TEXT_ANALYTICS_NAME -g $RESOURCE_GROUP_NAME --resource-type "Microsoft.CognitiveServices/accounts" --set identity="{\"type\": \"SystemAssigned\"}"
