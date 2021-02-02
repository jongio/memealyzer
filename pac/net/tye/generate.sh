#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

${ROOT}/scripts/k8sctx.sh

export ENV=cloud

source ./genfiles.sh

source $ROOT/scripts/login.sh

echo "GENERATING K8S MANIFEST FILES"
tye generate -v Debug --tags $ENV 