#!/bin/bash

export ROOT=../../../..;source $ROOT/scripts/base.sh
./push.sh
source ../shared/genfiles.sh
../shared/deploy.sh
./func.sh