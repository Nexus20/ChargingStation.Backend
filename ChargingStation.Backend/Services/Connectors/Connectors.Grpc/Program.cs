using Connectors.Grpc.Extensions;
using Connectors.Grpc.Services;
using ChargingStation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddConnectorGrpcServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ConnectorGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
