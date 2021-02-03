#!/bin/bash
set -euo pipefail

pushd ./iac/bicep > /dev/null
./provision.sh "$@"
popd > /dev/null