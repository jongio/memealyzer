FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
RUN apt-get update -y && \
    apt-get install python3 python3-distutils python3-pip -y && \
    pip3 install --user azure-cli
ENV PATH /root/.local/bin/:$PATH

WORKDIR /app

ENV ASPNETCORE_URLS http://+:80
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Api.dll"] 