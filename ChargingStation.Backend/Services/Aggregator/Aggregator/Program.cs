using Aggregator.Extensions;
using Aggregator.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAggregatorServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
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

app.MapControllers();

app.Run();
