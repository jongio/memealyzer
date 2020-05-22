FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/azsdkdemoapi/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY src/azsdkdemoapi/ ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
ENV ASPNETCORE_URLS http://+:80
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "azsdkdemoapi.dll"]