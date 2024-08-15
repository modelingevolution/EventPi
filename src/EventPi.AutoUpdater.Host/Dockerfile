# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update
RUN apt-get install -y ssh-client wget curl 

ENV DOCKER_VERSION=20.10.24
ENV DOCKER_COMPOSE_VERSION=2.29.1

# Add a script to download Docker binaries based on the architecture
RUN set -eux; \
    ARCH=$(uname -m); \
    if [ "$ARCH" = "x86_64" ]; then \
        ARCH="x86_64"; \
    elif [ "$ARCH" = "aarch64" ]; then \
        ARCH="aarch64"; \
    else \
        echo "Unsupported architecture: $ARCH"; \
        exit 1; \
    fi; \
    curl -fsSL "https://download.docker.com/linux/static/stable/$ARCH/docker-$DOCKER_VERSION.tgz" -o docker.tgz; \
    tar xzvf docker.tgz; \
    mv docker/* /usr/local/bin/; \
    rm -rf docker docker.tgz

# Add a script to download Docker Compose binaries based on the architecture
RUN set -eux; \
    ARCH=$(uname -m); \
    if [ "$ARCH" = "x86_64" ]; then \
        ARCH="x86_64"; \
    elif [ "$ARCH" = "aarch64" ]; then \
        ARCH="aarch64"; \
    else \
        echo "Unsupported architecture: $ARCH"; \
        exit 1; \
    fi; \
    curl -SL "https://github.com/docker/compose/releases/download/v${DOCKER_COMPOSE_VERSION}/docker-compose-$(uname -s)-${ARCH}" -o /usr/local/bin/docker-compose; \
    chmod +x /usr/local/bin/docker-compose

WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EventPi.AutoUpdater.Host/EventPi.AutoUpdater.Host.csproj", "EventPi.AutoUpdater.Host/"]
COPY ["EventPi.AutoUpdate/EventPi.AutoUpdate.csproj", "EventPi.AutoUpdate/"]
RUN dotnet restore "./EventPi.AutoUpdater.Host/EventPi.AutoUpdater.Host.csproj"
COPY . .
WORKDIR "/src/EventPi.AutoUpdater.Host"
RUN dotnet build "./EventPi.AutoUpdater.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EventPi.AutoUpdater.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventPi.AutoUpdater.Host.dll"]