# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies from the TaskManagerAPI folder
COPY TaskManagerAPI/*.csproj ./
RUN dotnet restore

# Copy everything else from TaskManagerAPI folder
COPY TaskManagerAPI/. ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published app
COPY --from=build /app/out ./

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Expose port
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "TaskManagerAPI.dll"]