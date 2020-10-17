#!/bin/bash
set -euo pipefail

az acr login --name ${AZURE_CONTAINER_REGISTRY_SERVER}
docker-compose -f ./docker-compose.yml build --parallel
docker-compose -f ./docker-compose.yml push