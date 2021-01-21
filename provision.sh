#!/bin/bash
set -euo pipefail

export ROOT=.;source $ROOT/scripts/base.sh

pushd ./iac/bicep > /dev/null
./provision.sh ${BASENAME}
popd > /dev/null