#!/bin/bash
set -euo pipefail

pushd ./iac > /dev/null
./provision.sh "$@"
popd > /dev/null