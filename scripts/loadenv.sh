#!/bin/bash
set -euo pipefail

export DOTENV_FILENAME=.env
export DOTENV=$ROOT/$DOTENV_FILENAME

echo "DOTENV=${DOTENV}"
[ -f $DOTENV ] && echo "DOTENV file at $DOTENV exists." || echo "DOTENV file at $DOTENV does not exist."

get_abs_filename() {
  # $1 : relative filename
  echo "$(cd "$(dirname "$1")" && pwd)/$(basename "$1")"
}

export DOTENV_FULLPATH=$(get_abs_filename $DOTENV)

echo "LOADING ENV VARS FROM: ${DOTENV}"
set -o allexport; source $DOTENV; set +o allexport