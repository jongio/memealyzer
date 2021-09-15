#!/bin/bash
set -euo pipefail

pushd ./pac > /dev/null
./run.sh "$@"
popd > /dev/null