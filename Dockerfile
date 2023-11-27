# Use an official Node.js runtime as the base image for the client build
FROM node:20 AS client-builder

# Set the working directory in the client builder container
WORKDIR /app/client

# Copy the client-side package.json and package-lock.json to the client builder container
COPY client/package*.json ./

# Install client application dependencies
RUN npm install

# Copy the client application source code to the client builder container
COPY client ./

# Build the client-side code
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app/server

# Copy everything
COPY server/*.csproj ./

# Restore as distinct layers
RUN dotnet restore

COPY server ./
COPY --from=client-builder /app/client/docs /app/server/wwwroot

# Build and publish a release
RUN dotnet publish -c Release -o out

# Start runtime image
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY --from=build-env /app/server/out .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet keepr.dll
