#!/bin/bash
set -euo pipefail

export ROOT=.;source $ROOT/scripts/base.sh

pushd iac/bicep
./deploy.sh ${BASENAME}
popd

pushd pac/net/tye
./deploy.sh ${BASENAME}
popd