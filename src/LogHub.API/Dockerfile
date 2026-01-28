# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY src/LogHub.Core/*.csproj LogHub.Core/
COPY src/LogHub.Infrastructure/*.csproj LogHub.Infrastructure/
COPY src/LogHub.API/*.csproj LogHub.API/

RUN dotnet restore LogHub.API/LogHub.API.csproj

# Copy source files and build
COPY src/LogHub.Core/ LogHub.Core/
COPY src/LogHub.Infrastructure/ LogHub.Infrastructure/
COPY src/LogHub.API/ LogHub.API/

WORKDIR /src/LogHub.API
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

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "LogHub.API.dll"]
