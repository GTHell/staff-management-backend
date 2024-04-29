# Use the official image as a parent image.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ./*.csproj ./
RUN dotnet restore

# Copy everything
COPY . ./

# Copy Staff.db with schema to the output directory
COPY Staffs.db /app/out/Staffs.db

# Resolve Test dependencies by reinit the project
RUN dotnet new sln -n StaffManagement --force
RUN dotnet sln add StaffManagement.csproj

# Build the project
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "StaffManagement.dll"]