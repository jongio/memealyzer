FROM mcr.microsoft.com/vscode/devcontainers/universal:1-linux
RUN dotnet tool install -g Microsoft.Tye --version "0.6.0-*" --add-source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet5/nuget/v3/index.json
USER root
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
     && apt-get -y install --no-install-recommends azure-functions-core-tools-3 \
     && curl -Lo /usr/local/bin/bicep https://github.com/Azure/bicep/releases/latest/download/bicep-linux-x64  && chmod +x /usr/local/bin/bicep 
USER codespace