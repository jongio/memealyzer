#!/bin/bash
set -euo pipefail

echo "LOADING ENV VARS"
set -o allexport; source ../../.env; set +o allexport

echo "RUNNING DOCKER COMPOSE UP"
docker-compose up
#docker-compose up --build
#docker-compose up --build memealyzernetwebapp memealyzernetapi memealyzernetqueueservice azurite
#docker-compose up memealyzernetapi azurite