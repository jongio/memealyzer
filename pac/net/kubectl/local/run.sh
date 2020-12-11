#!/bin/bash
set -euo pipefail


./mount.sh || true
export ROOT=../../../..;source $ROOT/scripts/base.sh
source ../shared/genfiles.sh
./build.sh
../shared/deploy.sh

($ROOT/scripts/azurite.sh & ./func.sh)