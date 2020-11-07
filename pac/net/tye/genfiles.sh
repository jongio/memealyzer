#!/bin/bash
#set -euo pipefail

echo "GENERATING TYE.YAML FILE"
TYE_YAML=${ROOT}/pac/net/tye/tye.yaml
TYE_YAML_TEMPLATE=${ROOT}/pac/net/tye/tye.template.yaml

envsubst < $TYE_YAML_TEMPLATE > $TYE_YAML