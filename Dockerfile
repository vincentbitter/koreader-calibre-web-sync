# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY Sync.csproj ./
RUN dotnet restore "Sync.csproj" --runtime linux-musl-x64
COPY . .
RUN dotnet publish -c Release -o out \
    --no-restore \  
    --runtime linux-musl-x64 \
    --self-contained true \
    /p:PublishTrimmed=true \
    /p:PublishSingleFile=true

# Run
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine AS final

RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app

USER dotnetuser
WORKDIR /app

EXPOSE 5000

COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://*:5000
CMD ./Sync