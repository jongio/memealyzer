#!/bin/bash
set -euo pipefail

pushd ./iac/bicep > /dev/null
./deprovision.sh "$@"
popd > /dev/null