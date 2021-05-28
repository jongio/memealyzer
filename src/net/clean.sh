#!/bin/bash 
set -euo pipefail

export ROOT=../..;

dotnet restore ${ROOT}/src/net/Api
dotnet clean ${ROOT}/src/net/Api
rm -rf ${ROOT}/src/net/Api/bin
rm -rf ${ROOT}/src/net/Api/obj

dotnet restore ${ROOT}/src/net/Lib
dotnet clean ${ROOT}/src/net/Lib
rm -rf ${ROOT}/src/net/Lib/bin
rm -rf ${ROOT}/src/net/Lib/obj

dotnet restore ${ROOT}/src/net/Lib.Model
dotnet clean ${ROOT}/src/net/Lib.Model
rm -rf ${ROOT}/src/net/Lib.Model/bin
rm -rf ${ROOT}/src/net/Lib.Model/obj

dotnet restore ${ROOT}/src/net/Services/Functions
dotnet clean ${ROOT}/src/net/Services/Functions
rm -rf ${ROOT}/src/net/Services/Functions/bin
rm -rf ${ROOT}/src/net/Services/Functions/obj

dotnet restore ${ROOT}/src/net/Services/QueueService
dotnet clean ${ROOT}/src/net/Services/QueueService
rm -rf ${ROOT}/src/net/Services/QueueService/bin
rm -rf ${ROOT}/src/net/Services/QueueService/obj

dotnet clean ${ROOT}/src/net/WebApp
rm -rf ${ROOT}/src/net/WebApp/bin
rm -rf ${ROOT}/src/net/WebApp/obj