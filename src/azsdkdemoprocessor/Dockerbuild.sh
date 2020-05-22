#!/bin/bash
mvn package assembly:single
docker build -t jongio/azsdkdemoprocessor:latest .