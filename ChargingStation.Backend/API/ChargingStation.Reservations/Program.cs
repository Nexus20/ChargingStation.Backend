using ChargingStation.Infrastructure;
using ChargingStation.InternalCommunication.Extensions;
using ChargingStation.Reservations.Extensions;
using ChargingStation.Reservations.Middlewares;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddReservationServices(builder.Configuration);
builder.Services.AddChargePointsGrpcClient(builder.Configuration);
builder.Services.AddOcppTagsGrpcClient(builder.Configuration);
builder.Services.AddConnectorsGrpcClient(builder.Configuration);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o =>
    o.AddPolicy("AllowAll", b => b
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
    ));

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.UseHangfireDashboard();

app.MapControllers();
app.MapHangfireDashboard();

app.Run();
