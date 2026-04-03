# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy everything
COPY src/ ./src/

# Restore and publish
RUN dotnet restore "src/Api/ToursApp.Api.csproj"
RUN dotnet publish "src/Api/ToursApp.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ToursApp.Api.dll"]