#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

source ./genfiles.sh

tye run --logs console -v debug --watch --tags local --debug memealyzernetqueueservice
