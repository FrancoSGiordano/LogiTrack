# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Fleet/Fleet.API/Fleet.API.csproj", "Services/Fleet/Fleet.API/"]
COPY ["Services/Fleet/Fleet.Application/Fleet.Application.csproj", "Services/Fleet/Fleet.Application/"]
COPY ["BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/BuildingBlocks/"]
COPY ["Services/Fleet/Fleet.Domain/Fleet.Domain.csproj", "Services/Fleet/Fleet.Domain/"]
COPY ["Services/Fleet/Fleet.Infrastructure/Fleet.Infrastructure.csproj", "Services/Fleet/Fleet.Infrastructure/"]
RUN dotnet restore "./Services/Fleet/Fleet.API/Fleet.API.csproj"
COPY . .
WORKDIR "/src/Services/Fleet/Fleet.API"
RUN dotnet build "./Fleet.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fleet.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fleet.API.dll"]