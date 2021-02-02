#!/bin/bash
set -euo pipefail

pushd ./pac/net/tye > /dev/null
./run.sh "$@"
popd > /dev/null