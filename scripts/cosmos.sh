#!/bin/bash
set -euo pipefail

if [ "${USE_COSMOS_EMULATOR:-}" ] && [ "$USE_COSMOS_EMULATOR" == "true" ]; then
    sudo apt-get update
    sudo apt-get install docker-ce docker-ce-cli containerd.io net-tools
    
    EXISTS=$(docker ps -qa -f name=cosmos-emulator)
    RUNNING=$(docker inspect --format="{{.State.Running}}" cosmos-emulator 2> /dev/null || true)
    if [ ! $EXISTS ] || [ "$RUNNING" == "false" ]; then
        echo "STARTING COSMOS EMULATOR"
        docker pull mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator
        docker start cosmos-emulator 2> /dev/null || docker run -d -p 8081:8081 -p 10251:10251 -p 10252:10252 -p 10253:10253 -p 10254:10254  -m 3g --cpus=2.0 --name=cosmos-emulator -e AZURE_COSMOS_EMULATOR_PARTITION_COUNT=10 -e AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE=true -e AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE=$IPADDR mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator  
        sleep 5
        while [ "$(docker logs -n 1 cosmos-emulator | cat -v)" != "Started^M" ]
        do
            echo "$(docker logs -n 1 cosmos-emulator)"
            sleep 2
        done
        
        CERT_FILE=~/emulatorcert.crt
        sudo curl -k https://$IPADDR:8081/_explorer/emulator.pem > $CERT_FILE
        sudo cp $CERT_FILE /usr/local/share/ca-certificates/
        pushd /usr/local/share/ca-certificates/ > /dev/null
        sudo update-ca-certificates
        popd > /dev/null
    fi  
fi