#!/bin/bash

BIN_FOLDER=templates/.bin

mkdir -p ./$BIN_FOLDER

echo "Replacing Environment Variables in Template Files"
for file in $(find templates/*.yaml -type f)
do
    envsubst < $file > $BIN_FOLDER/${file##*/}
done

echo "Switching K8S Context"
kubectl config use-context ${K8S_CONTEXT}

echo "Apply K8S Files"
kubectl apply -f ./$BIN_FOLDER/.