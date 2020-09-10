#!/bin/bash

echo "Switching K8S Context"
kubectl config use-context ${K8S_CONTEXT}

echo "Apply K8S Files"
kubectl apply -f ./$BIN_FOLDER/.