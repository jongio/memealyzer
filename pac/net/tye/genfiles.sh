#!/bin/bash
#set -euo pipefail

echo "GENERATING TYE.YAML FILE"
TYE_YAML=${ROOT}/pac/net/tye/tye.yaml
TYE_YAML_TEMPLATE=${ROOT}/pac/net/tye/tye.template.yaml
envsubst < $TYE_YAML_TEMPLATE > $TYE_YAML

echo "GENERATING .ENV FILE"
envsubst < ${ROOT}/${DOTENV_FILENAME} > ${DOTENV_FILENAME}

${ROOT}/scripts/genappsettings.sh

#$ROOT/src/net/clean.sh
