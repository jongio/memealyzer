#!/bin/bash
docker build -t jongio/azsdkdemoprocessor:latest .
docker run --rm -it --env-file=./../../.env jongio/azsdkdemoprocessor:latest