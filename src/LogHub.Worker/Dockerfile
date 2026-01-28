# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY src/LogHub.Core/*.csproj LogHub.Core/
COPY src/LogHub.Infrastructure/*.csproj LogHub.Infrastructure/
COPY src/LogHub.Worker/*.csproj LogHub.Worker/

RUN dotnet restore LogHub.Worker/LogHub.Worker.csproj

# Copy source files and build
COPY src/LogHub.Core/ LogHub.Core/
COPY src/LogHub.Infrastructure/ LogHub.Infrastructure/
COPY src/LogHub.Worker/ LogHub.Worker/

WORKDIR /src/LogHub.Worker
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user
RUN adduser --disabled-password --gecos "" appuser

COPY --from=build /app/publish .

# Change ownership
RUN chown -R appuser:appuser /app

USER appuser

ENV DOTNET_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "LogHub.Worker.dll"]
