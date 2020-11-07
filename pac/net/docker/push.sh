#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

az acr login --name ${AZURE_CONTAINER_REGISTRY_SERVER}
docker-compose build --parallel
docker-compose push