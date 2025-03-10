# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WeatherApp.API/WeatherApp.API.csproj", "WeatherApp.API/"]
COPY ["WeatherApp.Application/WeatherApp.Application.csproj", "WeatherApp.Application/"]
COPY ["WeatherApp.Domain/WeatherApp.Domain.csproj", "WeatherApp.Domain/"]
COPY ["WeatherApp.Infrastructure/WeatherApp.Infrastructure.csproj", "WeatherApp.Infrastructure/"]
RUN dotnet restore "./WeatherApp.API/WeatherApp.API.csproj"
COPY . .
WORKDIR "/src/WeatherApp.API"
RUN dotnet build "./WeatherApp.API.csproj" -c %BUILD_CONFIGURATION% -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WeatherApp.API.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherApp.API.dll"]