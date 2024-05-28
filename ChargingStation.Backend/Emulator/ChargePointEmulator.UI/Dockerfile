#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Emulator/ChargePointEmulator.UI/ChargePointEmulator.UI.csproj", "Emulator/ChargePointEmulator.UI/"]
COPY ["Emulator/ChargePointEmulator.Application/ChargePointEmulator.Application.csproj", "Emulator/ChargePointEmulator.Application/"]
COPY ["Domain/ChargingStation.Common/ChargingStation.Common.csproj", "Domain/ChargingStation.Common/"]
COPY ["Infrastructure/ChargingStation.Infrastructure/ChargingStation.Infrastructure.csproj", "Infrastructure/ChargingStation.Infrastructure/"]
COPY ["Domain/ChargingStation.Domain/ChargingStation.Domain.csproj", "Domain/ChargingStation.Domain/"]
COPY ["Emulator/ChargePointEmulator.Persistence/ChargePointEmulator.Persistence.csproj", "Emulator/ChargePointEmulator.Persistence/"]
RUN dotnet restore "./Emulator/ChargePointEmulator.UI/ChargePointEmulator.UI.csproj"
COPY . .
WORKDIR "/src/Emulator/ChargePointEmulator.UI"
RUN dotnet build "./ChargePointEmulator.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChargePointEmulator.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChargePointEmulator.UI.dll"]