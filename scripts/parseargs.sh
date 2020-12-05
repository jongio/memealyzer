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
export DOTENV_FILENAME=.env
export DOTENV=$ROOT/$DOTENV_FILENAME
get_abs_filename() {
  # $1 : relative filename
  echo "$(cd "$(dirname "$1")" && pwd)/$(basename "$1")"
}
export DOTENV_FULLPATH=get_abs_filename DOTENV

[ -f $DOTENV ] && echo "$DOTENV exists." || echo "$DOTENV does not exist."

echo "BASENAME=${BASENAME}"
echo "ENV=${ENV}"
echo "DOTENV=${DOTENV}"
