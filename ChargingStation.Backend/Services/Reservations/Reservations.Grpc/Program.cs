using ChargingStation.Infrastructure;
using Reservations.Grpc.Extensions;
using Reservations.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddReservationGrpcServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ReservationGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
