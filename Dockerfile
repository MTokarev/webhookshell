# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution file into the container
COPY *.sln ./

# Copy all project files into the container
COPY src/*.csproj ./src/

# Restore dependencies for the solution
RUN dotnet restore

# Copy all source code into the container
COPY src/ ./src/

# Build and publish the application
RUN dotnet publish -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install PowerShell and Python 3
# You can add or remove packages as needed
RUN apt-get update \
    && apt-get install -y \
    curl \
    gnupg \
    lsb-release \
    && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl https://packages.microsoft.com/config/debian/10/prod.list > /etc/apt/sources.list.d/microsoft-prod.list \
    && apt-get update \
    && apt-get install -y powershell \
    && apt-get install -y python3 python3-pip

# Copy the published output from the build stage
COPY --from=build /out ./

# Expose port and run the application
# 8080 - HTTP
# 8443 - HTTPS (you need to supply a certificate in the container)
EXPOSE 8080 8443
ENTRYPOINT ["dotnet", "Webhookshell.dll"]