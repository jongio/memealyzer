#!/bin/bash
set -euo pipefail

pushd ./pac > /dev/null
./debug.sh "$@"
popd > /dev/null