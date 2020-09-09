#!/bin/bash
echo "Loading Environment Variables"
set -o allexport; source ../../.env; set +o allexport

echo "Running Docker Compose Up"
docker-compose up
#docker-compose up --build
#docker-compose up --build azsdkdemonetwebapp azsdkdemonetapi azsdkdemonetqueueservice azurite
#docker-compose up azsdkdemonetapi azurite