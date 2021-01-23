#!/bin/bash
set -euo pipefail

if [ $# -eq 0 ]
  then
    echo "Please provide a basename to this script. i.e. script.sh memealyzerdev"
    exit
  else
    export BASENAME=$1
    if [ $# -eq 2 ]
      then
        export ENV=$2
      else
        export ENV=local
    fi
fi

echo "BASENAME=${BASENAME}"
echo "ENV=${ENV}"