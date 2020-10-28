#!/bin/bash
set -euo pipefail

echo "SWITCHING K8S CONTEXT"
kubectl config use-context ${K8S_CONTEXT}