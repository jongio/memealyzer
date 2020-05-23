#!/bin/bash
docker build -t azsdkdemoprocessor:latest .
docker run -it --env-file=../../.env -v c://Users/Jon/.azure:/home/developer/.azure azsdkdemoprocessor