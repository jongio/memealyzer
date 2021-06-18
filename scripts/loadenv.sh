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

# We only want to startup Ngrok container if Azurite is enabled.  "local" is the tag used by Tye and we ingect that into the tye.yaml
if [ "${USE_AZURITE:-}" == "true" ] || [ "${USE_AZURITE_BLOB:-}" == "true" ] || [ "${USE_AZURITE_QUEUE:-}" == "true" ] || [ "${USE_AZURITE_TABLE:-}" == "true" ]; then
  echo "AZURITE IS ENABLED. ENABLING TUNNEL..."
  export TUNNEL_ENABLED=local
else
  export TUNNEL_ENABLED=
fi