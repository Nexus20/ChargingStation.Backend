#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/ChargingProfiles/ChargingStation.ChargingProfiles/ChargingStation.ChargingProfiles.csproj", "Services/ChargingProfiles/ChargingStation.ChargingProfiles/"]
COPY ["Domain/ChargingStation.Common/ChargingStation.Common.csproj", "Domain/ChargingStation.Common/"]
COPY ["Infrastructure/ChargingStation.CacheManager/ChargingStation.CacheManager.csproj", "Infrastructure/ChargingStation.CacheManager/"]
COPY ["Infrastructure/ChargingStation.Infrastructure/ChargingStation.Infrastructure.csproj", "Infrastructure/ChargingStation.Infrastructure/"]
COPY ["Domain/ChargingStation.Domain/ChargingStation.Domain.csproj", "Domain/ChargingStation.Domain/"]
COPY ["Infrastructure/ChargingStation.InternalCommunication/ChargingStation.InternalCommunication.csproj", "Infrastructure/ChargingStation.InternalCommunication/"]
RUN dotnet restore "./Services/ChargingProfiles/ChargingStation.ChargingProfiles/ChargingStation.ChargingProfiles.csproj"
COPY . .
WORKDIR "/src/Services/ChargingProfiles/ChargingStation.ChargingProfiles"
RUN dotnet build "./ChargingStation.ChargingProfiles.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChargingStation.ChargingProfiles.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChargingStation.ChargingProfiles.dll"]