#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

${ROOT}/scripts/k8sctx.sh

source ./genfiles.sh

echo "UNDEPLOYING K8S"
tye undeploy -v Debug --tags $WORKSPACE
