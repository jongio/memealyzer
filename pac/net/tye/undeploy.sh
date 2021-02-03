#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

${ROOT}/scripts/k8sctx.sh

export ENV=cloud

source ./genfiles.sh

source $ROOT/scripts/login.sh

echo "UNDEPLOYING APPLICATION"

tye undeploy -v Debug --tags $ENV
