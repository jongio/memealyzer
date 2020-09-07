#!/bin/bash
set -o allexport; source ../../.env; set +o allexport
#docker-compose up --build
#docker-compose up --build azsdkdemonetwebapp azsdkdemonetapi azsdkdemonetqueueservice azurite
docker-compose up azsdkdemonetapi azurite