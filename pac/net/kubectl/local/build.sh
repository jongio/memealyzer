#!/bin/bash
set -euo pipefail

echo "RUNNING DOCKER COMPOSE"
docker-compose -f $ROOT/pac/net/docker/local/docker-compose.yml build 
