#!/bin/bash

export DOTENV_FILENAME=.env.prod
source ../shared/loadenv.sh
./push.sh
source ../shared/genfiles.sh
../shared/deploy.sh
./funcdeploy.sh