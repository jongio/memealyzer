#!/bin/bash
set -euo pipefail

pushd ./pac/net/tye > /dev/null
./deploy.sh "$@"
popd > /dev/null