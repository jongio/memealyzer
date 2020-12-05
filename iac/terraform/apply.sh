#!/bin/bash 
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

./workspace.sh

terraform apply tf.plan

source ./perms.sh ${BASENAME}