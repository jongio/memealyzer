#!/bin/bash
az acr login --name ${AZURE_CONTAINER_REGISTRY_SERVER}
docker-compose -f ../../../../src/net/docker-compose.yml build --parallel
docker-compose -f ../../../../src/net/docker-compose.yml push