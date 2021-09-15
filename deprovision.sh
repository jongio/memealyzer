#!/bin/bash
set -euo pipefail

pushd ./iac > /dev/null
./deprovision.sh "$@"
popd > /dev/null