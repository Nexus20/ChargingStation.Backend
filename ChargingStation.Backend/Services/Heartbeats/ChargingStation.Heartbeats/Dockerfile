#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Heartbeats/ChargingStation.Heartbeats/ChargingStation.Heartbeats.csproj", "Services/Heartbeats/ChargingStation.Heartbeats/"]
COPY ["Domain/ChargingStation.Common/ChargingStation.Common.csproj", "Domain/ChargingStation.Common/"]
COPY ["Infrastructure/ChargingStation.InternalCommunication/ChargingStation.InternalCommunication.csproj", "Infrastructure/ChargingStation.InternalCommunication/"]
COPY ["Infrastructure/ChargingStation.TableStorage/ChargingStation.TableStorage.csproj", "Infrastructure/ChargingStation.TableStorage/"]
RUN dotnet restore "./Services/Heartbeats/ChargingStation.Heartbeats/ChargingStation.Heartbeats.csproj"
COPY . .
WORKDIR "/src/Services/Heartbeats/ChargingStation.Heartbeats"
RUN dotnet build "./ChargingStation.Heartbeats.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChargingStation.Heartbeats.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChargingStation.Heartbeats.dll"]