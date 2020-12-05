#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

${ROOT}/scripts/k8sctx.sh

export ENV=cloud

source ./genfiles.sh

echo "ACR LOGIN"
az acr login --name ${AZURE_CONTAINER_REGISTRY_SERVER}

echo "DEPLOY INGRESS"
kubectl apply -f https://aka.ms/tye/ingress/deploy

echo "TYE DEPLOY"
tye deploy -v Debug --tags $ENV

${ROOT}/scripts/func.sh
${ROOT}/scripts/ingressip.sh