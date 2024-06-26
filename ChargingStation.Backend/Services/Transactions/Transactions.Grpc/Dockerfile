#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Transactions/Transactions.Grpc/Transactions.Grpc.csproj", "Services/Transactions/Transactions.Grpc/"]
COPY ["Services/Transactions/Transactions.Application/Transactions.Application.csproj", "Services/Transactions/Transactions.Application/"]
COPY ["Infrastructure/ChargingStation.Infrastructure/ChargingStation.Infrastructure.csproj", "Infrastructure/ChargingStation.Infrastructure/"]
COPY ["Domain/ChargingStation.Common/ChargingStation.Common.csproj", "Domain/ChargingStation.Common/"]
COPY ["Domain/ChargingStation.Domain/ChargingStation.Domain.csproj", "Domain/ChargingStation.Domain/"]
COPY ["Infrastructure/ChargingStation.InternalCommunication/ChargingStation.InternalCommunication.csproj", "Infrastructure/ChargingStation.InternalCommunication/"]
COPY ["Infrastructure/ChargingStation.Mailing/ChargingStation.Mailing.csproj", "Infrastructure/ChargingStation.Mailing/"]
RUN dotnet restore "./Services/Transactions/Transactions.Grpc/Transactions.Grpc.csproj"
COPY . .
WORKDIR "/src/Services/Transactions/Transactions.Grpc"
RUN dotnet build "./Transactions.Grpc.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Transactions.Grpc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Transactions.Grpc.dll"]