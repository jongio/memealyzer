FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "Services/QueueService/QueueService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "QueueService.dll"]
