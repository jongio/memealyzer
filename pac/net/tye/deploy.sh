#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

${ROOT}/scripts/k8sctx.sh

source ./genfiles.sh

echo "ACR LOGIN"
az acr login --name ${AZURE_CONTAINER_REGISTRY_SERVER}

echo "TYE DEPLOY"
tye deploy -v Debug --tags cloud --interactive

echo "DEPLOY FUNCTION"
${ROOT}/pac/net/kubectl/aks/func.sh

IP_ADDRESS=$(kubectl get ing -o=jsonpath='{.items[0].status.loadBalancer.ingress[0].ip}')
echo "SITE LOCATION: http://${IP_ADDRESS}"