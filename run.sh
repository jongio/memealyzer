#!/bin/bash
set -euo pipefail

export ROOT=.;source $ROOT/scripts/base.sh

pushd ./pac/net/tye > /dev/null
./run.sh ${BASENAME}
popd > /dev/null