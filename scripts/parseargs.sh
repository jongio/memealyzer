#!/bin/bash
set -euo pipefail

if [ $# -eq 0 ] || [ "$1" == "dev" ]
  then
    export DOTENV_FILENAME=.env
    export WORKSPACE=dev
  else
    export DOTENV_FILENAME=.env.$1
    export WORKSPACE=$1
fi

export DOTENV=$ROOT/$DOTENV_FILENAME
[ -f $DOTENV ] && echo "$DOTENV exists." || echo "$DOTENV does not exist."

echo "WORKSPACE=${WORKSPACE}"
echo "DOTENV=${DOTENV}"