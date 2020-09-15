#!/bin/bash

source ../../scripts/loadenv.sh
terraform workspace select $WORKSPACE || terraform workspace new $WORKSPACE
terraform workspace select $WORKSPACE
terraform plan --out=tf.plan