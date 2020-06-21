FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "Api/Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
RUN apt-get update -y && apt-get install python3 python3-distutils python3-pip -y && pip3 install azure-cli
ENV PATH /root/.local/bin/:$PATH

WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Api.dll"]