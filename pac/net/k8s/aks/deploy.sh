#!/bin/bash

export DOTENV_FILENAME=.env.prod
source ../shared/loadenv.sh
./push.sh
../shared/deploy.sh
./funcdeploy.sh