#!/bin/bash
docker build -t azsdkdemoapi:latest .
docker run -it --env-file=../../.env -p 5051:80 -v c://Users/Jon/.azure:/root/.azure azsdkdemoapi