FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out



# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App

ENV DELUGE_URL="http://192.168.0.118:9090/json"
ENV DELUGE_PASS="admin"

EXPOSE 80

COPY --from=build-env /App/out .
ENTRYPOINT ["./DelugeClient", "--urls http://*:80 --deluged-host ${DELUGE_URL} --deluged-password ${DELUGE_PASS}"]