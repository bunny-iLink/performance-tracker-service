# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . ./
RUN dotnet restore

# Build and publish the app
RUN dotnet publish -c Release -o /app/out

# Stage 2: Create runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Install 'procps' to enable use of the 'free' command for memory metrics
RUN apt-get update && apt-get install -y procps && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy published files from build container
COPY --from=build /app/out .

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true

# Expose the port Render will listen on
EXPOSE 8080

# Start the app
ENTRYPOINT ["dotnet", "backend.dll"]
