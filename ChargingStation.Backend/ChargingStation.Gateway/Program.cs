using ChargingStation.Gateway.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

if (builder.Configuration["DOCKER_ENV"] == "true")
{
    builder.Configuration.AddOcelotJsonFiles("ocelot_for_docker");
}
else
    builder.Configuration.AddOcelotJsonFiles("OcelotSettings");

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

await app.UseOcelot();

app.Run();

