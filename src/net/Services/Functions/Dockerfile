FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS installer-env

COPY . /src/dotnet-function-app
RUN cd /src/dotnet-function-app && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish "Services/Functions/Functions.csproj" --output /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
#FROM mcr.microsoft.com/azure-functions/dotnet:3.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet:3.0
RUN apt-get update && apt-get install -y curl && curl -sL https://aka.ms/InstallAzureCLIDeb | bash
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true \
    CORS_ALLOWED_ORIGINS=* \
    CORS_SUPPORT_CREDENTIALS=false 

ENV AZURE_FUNCTIONS_ENVIRONMENT Development

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]