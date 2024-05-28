#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/ChargePoints/ChargePoints.Api/ChargePoints.Api.csproj", "Services/ChargePoints/ChargePoints.Api/"]
COPY ["Infrastructure/ChargingStation.InternalCommunication/ChargingStation.InternalCommunication.csproj", "Infrastructure/ChargingStation.InternalCommunication/"]
COPY ["Domain/ChargingStation.Common/ChargingStation.Common.csproj", "Domain/ChargingStation.Common/"]
COPY ["Services/ChargePoints/ChargePoints.Application/ChargePoints.Application.csproj", "Services/ChargePoints/ChargePoints.Application/"]
COPY ["Infrastructure/ChargingStation.CacheManager/ChargingStation.CacheManager.csproj", "Infrastructure/ChargingStation.CacheManager/"]
COPY ["Infrastructure/ChargingStation.Infrastructure/ChargingStation.Infrastructure.csproj", "Infrastructure/ChargingStation.Infrastructure/"]
COPY ["Domain/ChargingStation.Domain/ChargingStation.Domain.csproj", "Domain/ChargingStation.Domain/"]
RUN dotnet restore "./Services/ChargePoints/ChargePoints.Api/ChargePoints.Api.csproj"
COPY . .
WORKDIR "/src/Services/ChargePoints/ChargePoints.Api"
RUN dotnet build "./ChargePoints.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChargePoints.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChargePoints.Api.dll"]