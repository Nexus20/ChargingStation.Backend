using ChargingStation.WebSockets.Extensions;
using ChargingStation.WebSockets.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOcppCommunicationServices(builder.Configuration);
builder.Services.AddControllers();
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
// Set WebSocketsOptions
var webSocketOptions = new WebSocketOptions();

// Accept WebSocket
app.UseWebSockets(webSocketOptions);
app.UseMiddleware<OcppWebSocketMiddleware>();

app.Run();
