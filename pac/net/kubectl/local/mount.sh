#!/bin/bash
set -euo pipefail

mkdir -p /mnt/wsl/.azure
sudo mount --bind ${HOME}/.azure /mnt/wsl/.azure
echo "MOUNTED .azure FOLDER"