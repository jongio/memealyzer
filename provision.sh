#!/bin/bash
set -euo pipefail

pushd ./iac/bicep > /dev/null
./provision.sh $1
popd > /dev/null