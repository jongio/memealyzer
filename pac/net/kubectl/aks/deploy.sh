#!/bin/bash
set -euo pipefail

export ROOT=../../../..;source $ROOT/scripts/base.sh
./push.sh
source ../shared/genfiles.sh
../shared/deploy.sh
${ROOT}/scripts/func.sh
${ROOT}/scripts/ingressip.sh