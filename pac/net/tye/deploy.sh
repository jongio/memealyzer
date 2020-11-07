#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

${ROOT}/scripts/k8sctx.sh

source ./genfiles.sh

echo "ACR LOGIN"
az acr login --name ${AZURE_CONTAINER_REGISTRY_SERVER}

echo "TYE DEPLOY"
tye deploy -v Debug --tags $WORKSPACE

echo "DEPLOY FUNCTION"
${ROOT}/pac/net/kubectl/aks/func.sh