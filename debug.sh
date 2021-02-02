#!/bin/bash
set -euo pipefail

pushd ./pac/net/tye > /dev/null
./debug.sh $1
popd > /dev/null