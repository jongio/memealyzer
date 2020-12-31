#!/bin/bash
set -euo pipefail

INSIDERS=""
if [ $# -eq 1 ] && [ $1 = "insiders" ]; then
   INSIDERS="-insiders"
fi
echo ${INSIDERS}
code${INSIDERS} src/net/memealyzer.code-workspace