#!/bin/bash
#set -euo pipefail

echo "GENERATING TYE.YAML FILE"
envsubst < tye.template.yaml > tye.yaml