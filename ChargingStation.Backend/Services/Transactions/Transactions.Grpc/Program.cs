using ChargingStation.Infrastructure;
using Transactions.Grpc.Extensions;
using Transactions.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddTransactionGrpcServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TransactionGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
