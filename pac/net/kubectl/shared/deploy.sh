#!/bin/bash
set -euo pipefail

${ROOT}/scripts/k8sctx.sh

echo "APPLY K8S FILES"
kubectl apply -f ./$BIN_FOLDER/.