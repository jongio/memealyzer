#!/bin/bash

echo "####"
echo "For local deployment, you also need to run /pac/net/k8s/local/mount.sh"
echo "####"

export DOTENV_FILENAME=.env
source ../shared/loadenv.sh
source ../shared/genfiles.sh
./build.sh
../shared/deploy.sh

(./azuritestart.sh & ./funcstart.sh)