#!/bin/bash
#set -euo pipefail

echo "GENERATE FILES"

export TYE_ROOT=../../../../..
export BIN_FOLDER=.bin
export TYE_FILE=$BIN_FOLDER/tye.yaml

echo "TYE_FILE="$TYE_FILE

mkdir -p ./$BIN_FOLDER
envsubst < tye.yaml > $TYE_FILE

APPSETTINGS=${ROOT}/src/net/WebApp/wwwroot/appsettings.json
envsubst < $APPSETTINGS.temp > $APPSETTINGS

# Issue: We need this file because tye and appsettings.json do not support env var replacements
# https://github.com/dotnet/tye/issues/223
# Discuss: Discuss Env var expansion in appsettings.json