using ChargingStation.SignalR.Extensions;
using ChargingStation.SignalR.Hubs;
using ChargingStation.SignalR.Middlewares;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalRServices(builder.Configuration);
builder.Services.AddCors(o =>
    o.AddPolicy("AllowAll", b => b
        .WithOrigins("http://localhost:4200", "https://ev-charging-station.onrender.com", "https://ev-charging-proxy.onrender.com")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
    ));


// Configure the HTTP request pipeline.
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();

app.MapHub<ChargingStationHub>("/ChargePointHub");
app.MapControllers();

app.Run();
