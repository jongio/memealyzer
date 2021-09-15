#!/bin/bash
set -euo pipefail

pushd ./pac > /dev/null
./deploy.sh "$@"
popd > /dev/null