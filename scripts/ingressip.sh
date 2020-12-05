#!/bin/bash
set -euo pipefail

IP_ADDRESS=$(kubectl get ing -o=jsonpath='{.items[0].status.loadBalancer.ingress[0].ip}')
echo "SITE LOCATION: http://${IP_ADDRESS}"