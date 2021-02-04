#!/bin/bash
set -euo pipefail

if [[ "${CLOUDENV_ENVIRONMENT_ID:-}" ]]; then
    export API_ENDPOINT=https://${CLOUDENV_ENVIRONMENT_ID}-2080.apps.codespaces.githubusercontent.com
    export FUNCTIONS_ENDPOINT=https://${CLOUDENV_ENVIRONMENT_ID}-3080.apps.codespaces.githubusercontent.com
elif [ "$ENV" = "local" ]; then
    export FUNCTIONS_ENDPOINT=
else
    export FUNCTIONS_ENDPOINT=http://${BASENAME}function.azurewebsites.net
fi