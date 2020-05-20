#!/bin/bash

# baseName location assetsBaseUrl

az deployment sub create -l $2 -u "$3/azuredeploy.all.json" --parameters baseName=$1

./localUserRoleAssignment.sh