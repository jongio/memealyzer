#!/bin/bash
set -euo pipefail

export ROOT=../../../..;source $ROOT/scripts/base.sh

source ../shared/genfiles.sh
${ROOT}/pac/net/kubectl/shared/k8sctx.sh

echo "GENERATING K8S MANIFEST FILES"
tye generate $TYE_FILE -v Debug --tags $WORKSPACE 

# Issue: We filed this issue about wanting manifest files because didn't know this generate command exists
# https://github.com/dotnet/tye/issues/718 (closed)
# Issue to document generate command: https://github.com/dotnet/tye/issues/730