#!/bin/bash
set -euo pipefail

echo "GETTING AKS CREDENTIALS"
az aks get-credentials --resource-group ${BASENAME}rg --name ${BASENAME}aks --overwrite-existing

echo "SWITCHING K8S CONTEXT"
kubectl config use-context ${K8S_CONTEXT}