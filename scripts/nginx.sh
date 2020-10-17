#!/bin/bash
set -euo pipefail

helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx && helm repo add stable https://kubernetes-charts.storage.googleapis.com/ && helm repo update && helm install nginx ingress-nginx/ingress-nginx

echo "If this script throws an Error: cannot re-use a name that is still in use, then run an upgrade instead of an install."
# If you already have installed or get this error: Error: cannot re-use a name that is still in use
# helm upgrade --install nginx ingress-nginx/ingress-nginx
