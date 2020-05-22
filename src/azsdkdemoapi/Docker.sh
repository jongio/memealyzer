#!/bin/bash
docker build -t jongio/azsdkdemoapi:latest .
docker run -d -p 8080:80 --name azsdkdemoapi --env-file=./../../.env jongio/azsdkdemoapi:latest
