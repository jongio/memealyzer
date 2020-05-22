#!/bin/bash
docker build -t azsdkdemoprocessor:latest .
docker run --rm -it --env-file=./../../.env azsdkdemoprocessor:latest