#!/bin/bash
set -euo pipefail

echo "LOADING ENV VARS FROM: ${DOTENV}"
set -o allexport; source $DOTENV; set +o allexport