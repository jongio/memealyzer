#!/bin/bash
set -euo pipefail

# Make sure .env file has intended value in AZURE_CONTAINER_REGISTRY_SERVER
set -o allexport; source ../../.env; set +o allexport
docker-compose build --parallel
docker-compose push