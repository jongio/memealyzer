#!/bin/bash
set -euo pipefail

if [ "$ENV" = "local" ]
then
    export FUNCTIONS_ENDPOINT=
else
    export FUNCTIONS_ENDPOINT=https://${BASENAME}function.azurewebsites.net
fi