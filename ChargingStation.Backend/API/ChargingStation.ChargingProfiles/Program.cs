using ChargingStation.CacheManager.Extensions;
using ChargingStation.ChargingProfiles.Extensions;
using ChargingStation.ChargingProfiles.Middlewares;
using ChargingStation.Infrastructure;
using ChargingStation.InternalCommunication.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddCacheServices(builder.Configuration);
builder.Services.AddChargingProfilesServices(builder.Configuration);
builder.Services.AddChargePointsGrpcClient(builder.Configuration);
builder.Services.AddConnectorsGrpcClient(builder.Configuration);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
