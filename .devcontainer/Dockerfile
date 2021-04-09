FROM mcr.microsoft.com/vscode/devcontainers/universal:1.0-linux
RUN dotnet tool install -g Microsoft.Tye --version "0.7.0-*" --add-source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet5/nuget/v3/index.json
USER root
RUN apt-get update && apt-get install -y --no-install-recommends apt-utils && apt-get install -y curl unzip procps && curl -sSL https://aka.ms/InstallAzureCLIDeb | bash
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
     && apt-get -y install --no-install-recommends azure-functions-core-tools-3 \
     && az bicep install 
USER codespace