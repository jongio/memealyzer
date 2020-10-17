#!/bin/bash
echo "Loading Environment Variables"
set -o allexport; source ../../.env; set +o allexport

echo "Running Docker Compose Up"
docker-compose up
#docker-compose up --build
#docker-compose up --build memealyzernetwebapp memealyzernetapi memealyzernetqueueservice azurite
#docker-compose up memealyzernetapi azurite