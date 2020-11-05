#!/bin/bash
set -euo pipefail

export ROOT=../../..;source $ROOT/scripts/base.sh

func start --script-root ${ROOT}/src/net/Services/Functions