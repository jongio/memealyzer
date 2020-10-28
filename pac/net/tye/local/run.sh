#!/bin/bash
set -euo pipefail

export ROOT=../../../..;source $ROOT/scripts/base.sh
source ../shared/genfiles.sh

tye run $TYE_FILE --logs console -v debug --watch --tags ${WORKSPACE}  #--debug memealyzernetapi

# Issue: Even though we have --logs console here, the logs from the QueueService (Console.WriteLine) are not being written to console.
# https://github.com/dotnet/tye/issues/711

