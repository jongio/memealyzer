FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app
COPY . ./
RUN dotnet publish "WebApp/WebApp.csproj" -c Release -o output

FROM nginx:alpine
WORKDIR /var/www/web
COPY --from=build /app/output/wwwroot .
COPY WebApp/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
EXPOSE 443