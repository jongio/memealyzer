#!/bin/bash
set -euo pipefail

helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx 
helm repo add stable https://kubernetes-charts.storage.googleapis.com/
helm repo update
helm upgrade --install nginx ingress-nginx/ingress-nginx