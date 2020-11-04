#!/bin/bash
set -euo pipefail

export ROOT=../../../..;source $ROOT/scripts/base.sh
source ../shared/genfiles.sh

tye run $TYE_FILE --logs console -v debug --watch --tags ${WORKSPACE}  #--debug memealyzernetapi
