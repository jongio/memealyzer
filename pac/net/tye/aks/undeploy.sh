#!/bin/bash
set -euo pipefail

export ROOT=../../../..;source $ROOT/scripts/base.sh

source ../shared/gen.sh
${ROOT}/pac/net/kubectl/shared/k8sctx.sh

echo "UNDEPLOYING K8S"
tye undeploy -v Debug --tags $WORKSPACE $TYE_FILE
