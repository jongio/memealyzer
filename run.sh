#!/bin/bash
set -euo pipefail

pushd ./pac/net/tye > /dev/null
./run.sh $1
popd > /dev/null