# Issue: The container generated for Blazor projects does not work with tye deploy
# I created this special nginx container with the exact name "Dockerfile" because tye uses that file if it exists
# https://github.com/dotnet/tye/issues/720
FROM nginx:alpine
WORKDIR /var/www/web
COPY ./wwwroot .
COPY wwwroot/_framework/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
EXPOSE 443

