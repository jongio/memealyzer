#!/bin/bash 
set -euo pipefail

sudo apt-get update
sudo apt install wget libnss3-tools -y
export VER="v1.3.0"
wget -O mkcert https://github.com/FiloSottile/mkcert/releases/download/${VER}/mkcert-${VER}-linux-amd64
chmod +x  mkcert
sudo mv mkcert /usr/local/bin
mkcert -install
mkcert 127.0.0.1
sudo mv 127.0.0.1-key.pem /usr/local/bin
sudo mv 127.0.0.1.pem /usr/local/bin