# Get base image
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy the project file and restore any dependencies (through nugget)
COPY *.csproj ./
RUN dotnet restore

# Copy the project files and build our release
COPY . ./
RUN dotnet publish -c Release -o out

# Generate runtime image - don't use sdk
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
EXPOSE 5000
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "gateway.dll" ]
