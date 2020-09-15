#!/bin/bash

echo "Running Docker Compose"
docker-compose -f $ROOT/src/net/docker-compose.yml build 
