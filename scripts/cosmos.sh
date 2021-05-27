#!/bin/bash
set -euo pipefail

if [ "${USE_COSMOS_EMULATOR:-}" ] && [ "$USE_COSMOS_EMULATOR" == "true" ]; then

    echo "STARTING COSMOS EMULATOR"

    sudo apt-get update
    sudo apt-get install docker-ce docker-ce-cli containerd.io net-tools

    EXISTS=$(docker ps -qa -f name=cosmos-emulator)
    RUNNING=$(docker inspect --format="{{.State.Running}}" cosmos-emulator 2> /dev/null || true)
    
    # If the image doesn't exist or isn't running, then let's get it runnin.
    if [ ! $EXISTS ] || [ "$RUNNING" == "false" ]; then
        
        # Explicitly pull the image because it takes a while and the subsequent commands won't need to sleep as long or poll 
        docker pull mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator
        
        # First try to start the container, if that fails then run it
        docker start cosmos-emulator 2> /dev/null || docker run -d -p 8081:8081 -p 10251:10251 -p 10252:10252 -p 10253:10253 -p 10254:10254  -m 3g --cpus=2.0 --name=cosmos-emulator -e AZURE_COSMOS_EMULATOR_PARTITION_COUNT=10 -e AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE=true -e AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE=$IPADDR mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator  
        
        # 5 second delay here because previous log ends with "Started". This allows the new instance of the container to write a new and different line.
        sleep 5

        # Loop through the docker logs until the last line is "Started".  The cat -v is here because of the ^M hidden char
        while [ "$(docker logs -n 1 cosmos-emulator | cat -v)" != "Started^M" ]
        do
            echo "$(docker logs -n 1 cosmos-emulator)"
            sleep 2
        done
        
        # Now that the emulator has started we can get the certs installed.
        CERT_FILE=~/emulatorcert.crt
        sudo curl -k https://$IPADDR:8081/_explorer/emulator.pem > $CERT_FILE
        sudo cp $CERT_FILE /usr/local/share/ca-certificates/
        pushd /usr/local/share/ca-certificates/ > /dev/null
        sudo update-ca-certificates
        popd > /dev/null
    fi  
fi