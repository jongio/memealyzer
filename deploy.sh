#!/bin/bash
set -euo pipefail

pushd pac/net/tye > /dev/null
./deploy.sh $1
popd > /dev/null