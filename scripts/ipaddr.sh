#!/bin/bash
set -euo pipefail

export IPADDR=$(ifconfig | grep "inet " | grep -Fv 127.0.0.1 | awk '{print $2}' | head -n 1)
#echo $IPADDR