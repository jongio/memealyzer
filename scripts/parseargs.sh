#!/bin/bash
set -euo pipefail

# If user doesn't supply any parameters, then see if we have BASENAME environment var, if so use it, if not prompt

if [ $# -eq 0 ] 
then
    if [[ -z "${BASENAME:-}" ]]; 
    then
      read -r -p "Enter a basename value for all Azure resources, such as \`memealyzerdev\`: " response
    
      if [ -z "$response" ] 
      then
        echo "You didn't enter a value. Please re-run the script."
        exit
      else
        export BASENAME=$response
      fi
    fi
else
  export BASENAME=$1
fi

if [ $# -eq 2 ] 
then
  export ENV=$2
else
  export ENV=local
fi

echo "BASENAME=${BASENAME}"
echo "ENV=${ENV}"