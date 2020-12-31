#!/bin/bash
set -euo pipefail

export ROOT=.;source $ROOT/scripts/base.sh

pushd ./iac/bicep
./provision.sh ${BASENAME}
popd