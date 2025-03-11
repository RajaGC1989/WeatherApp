
FROM mcr.microsoft.com/dotnet/aspnet:9.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


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

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WeatherApp.API.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherApp.API.dll"]