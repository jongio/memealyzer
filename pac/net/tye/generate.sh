#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

${ROOT}/scripts/k8sctx.sh

source ./genfiles.sh

echo "GENERATING K8S MANIFEST FILES"
tye generate -v Debug --tags cloud 