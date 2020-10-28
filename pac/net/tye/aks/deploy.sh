#!/bin/bash
set -euo pipefail

export ROOT=../../../..;source $ROOT/scripts/base.sh

echo "ACR LOGIN"
az acr login --name ${AZURE_CONTAINER_REGISTRY_SERVER}

source ../shared/genfiles.sh
${ROOT}/pac/net/kubectl/shared/k8sctx.sh

echo "TYE DEPLOY"
tye deploy $TYE_FILE -v Debug --tags $WORKSPACE

echo "K8S APPLY TWEAKS"
kubectl apply -f ./templates/.

echo "DEPLOY FUNCTION"
${ROOT}/pac/net/kubectl/aks/func.sh

# Issue: Deploy will use Dockerfile if it's found without being told to use it.  So I had to rename all of my Dockerfiles to Dockerfile.{moniker}
# https://github.com/dotnet/tye/issues/714