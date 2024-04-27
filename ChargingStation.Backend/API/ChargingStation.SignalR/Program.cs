using ChargingStation.SignalR.Extensions;
using ChargingStation.SignalR.Hubs;
using ChargingStation.SignalR.Middlewares;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalRServices(builder.Configuration);


// Configure the HTTP request pipeline.
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapHub<ChargingStationHub>("/ChargePointHub");
app.MapControllers();

app.Run();
