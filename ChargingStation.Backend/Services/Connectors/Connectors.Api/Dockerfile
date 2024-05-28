#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Connectors/Connectors.Api/Connectors.Api.csproj", "Services/Connectors/Connectors.Api/"]
COPY ["Services/Connectors/Connectors.Application/Connectors.Application.csproj", "Services/Connectors/Connectors.Application/"]
COPY ["Infrastructure/ChargingStation.CacheManager/ChargingStation.CacheManager.csproj", "Infrastructure/ChargingStation.CacheManager/"]
COPY ["Infrastructure/ChargingStation.Infrastructure/ChargingStation.Infrastructure.csproj", "Infrastructure/ChargingStation.Infrastructure/"]
COPY ["Domain/ChargingStation.Common/ChargingStation.Common.csproj", "Domain/ChargingStation.Common/"]
COPY ["Domain/ChargingStation.Domain/ChargingStation.Domain.csproj", "Domain/ChargingStation.Domain/"]
COPY ["Infrastructure/ChargingStation.InternalCommunication/ChargingStation.InternalCommunication.csproj", "Infrastructure/ChargingStation.InternalCommunication/"]
RUN dotnet restore "./Services/Connectors/Connectors.Api/Connectors.Api.csproj"
COPY . .
WORKDIR "/src/Services/Connectors/Connectors.Api"
RUN dotnet build "./Connectors.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Connectors.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Connectors.Api.dll"]