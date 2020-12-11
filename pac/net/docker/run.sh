#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

echo "Running docker-compose build & up, starting azurite. You need to manually run ./func.sh"
docker-compose build --parallel; docker-compose up --remove-orphans
#docker-compose up
#docker-compose up --build
#docker-compose up --build memealyzernetqueueservice azurite
#docker-compose up azurite memealyzernetqueueservice memealyzernetapi