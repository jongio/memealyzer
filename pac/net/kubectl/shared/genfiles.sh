#!/bin/bash
set -euo pipefail

export BIN_FOLDER=templates/.bin

mkdir -p ./$BIN_FOLDER

echo "REPLACING ENV VAR IN TEMPLATE FILES"
for file in $(find templates/*.yaml -type f)
do
    envsubst < $file > $BIN_FOLDER/${file##*/}
done