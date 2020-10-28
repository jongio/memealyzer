#!/bin/bash
set -euo pipefail

../shared/k8sctx.sh

echo "APPLY K8S FILES"
kubectl apply -f ./$BIN_FOLDER/.