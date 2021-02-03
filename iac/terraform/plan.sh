#!/bin/bash
set -euo pipefail

export ROOT=../..;source $ROOT/scripts/base.sh

./workspace.sh

terraform plan --out=tf.plan