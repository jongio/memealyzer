#!/bin/bash
set -euo pipefail

pushd ./pac/net/tye > /dev/null
./debug.sh "$@"
popd > /dev/null