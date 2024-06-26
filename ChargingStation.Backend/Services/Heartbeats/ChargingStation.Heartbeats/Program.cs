using ChargingStation.Heartbeats.Extensions;
using ChargingStation.Heartbeats.Middlewares;
using ChargingStation.InternalCommunication.Extensions;

// Add services to the container.

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddHeartbeatServices(builder.Configuration);
builder.Services.AddChargePointsGrpcClient(builder.Configuration);
builder.Services.AddCustomSwaggerGen();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(o =>
    o.AddPolicy("AllowAll", b => b
        .WithOrigins("http://localhost:4200")
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

app.MapControllers();

app.Run();