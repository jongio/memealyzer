#!/bin/bash
echo "Loading Environment Variables"
set -o allexport; source ../../.env; set +o allexport

echo "Running Docker Compose Up"
docker-compose up --build
#docker-compose up --build
#docker-compose up --build azsdkdemonetqueueservice azurite
#docker-compose up azurite azsdkdemonetqueueservice azsdkdemonetapi