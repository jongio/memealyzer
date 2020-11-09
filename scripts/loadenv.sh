#!/bin/bash
set -euo pipefail

#echo "LOADING ENV VARS FROM: ${DOTENV}"

export DOTENV=.env
set -o allexport; source $DOTENV; set +o allexport

echo $AZURE_FUNCTION_APP_NAME