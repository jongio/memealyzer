#!/bin/bash

export ROOT=../..;source $ROOT/scripts/base.sh

terraform workspace select $WORKSPACE || terraform workspace new $WORKSPACE
terraform workspace select $WORKSPACE
terraform plan --out=tf.plan