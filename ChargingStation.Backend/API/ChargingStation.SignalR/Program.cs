using ChargingStation.SignalR.Extensions;
using ChargingStation.SignalR.Hubs;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalRServices(builder.Configuration);


// Configure the HTTP request pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapHub<ChargingStationHub>("/ChargePointHub");
app.MapControllers();

app.Run();
