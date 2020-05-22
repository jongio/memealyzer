#!/bin/bash
docker build -t azsdkdemoapi:latest .
docker run -d -p 8080:80 --name azsdkdemoapi --env-file=./../../.env azsdkdemoapi:latest
