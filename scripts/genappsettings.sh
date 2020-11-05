#!/bin/bash
#set -euo pipefail

echo "GENERATING APPSETTINGS.JSON FILE"
APPSETTINGS=${ROOT}/src/net/WebApp/wwwroot/appsettings.json
APPSETTINGS_TEMPLATE=${ROOT}/src/net/WebApp/wwwroot/appsettings.template.json

envsubst < $APPSETTINGS_TEMPLATE > $APPSETTINGS