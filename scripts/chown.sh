#!/bin/bash
set -euo pipefail

# Run this manually from the /scripts folder.

export ROOT=..;

echo "ADJUSTING FILE PERMISSIONS"

# You may need to run this in your devcontainer if you get build errors.

# Use this if you don't have $ROOT and are running in codespace
# sudo chown -R codespace /home/codespace/workspace/src

# Use the for all files
# sudo chown -v -R $USER $ROOT

# Use this for only dll & CopyComplete files
find $ROOT -type f \( -name '*.dll' -o -name '*.CopyComplete' \) | xargs sudo chown -v -R $USER