using ChargePointEmulator.Application.Extensions;
using ChargePointEmulator.Persistence;
using ChargePointEmulator.UI.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddStateManagementRepository(builder.Configuration);
builder.Services.AddChargePointServices(builder.Configuration);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapHub<ChargePointHub>("/ChargePointHub");
app.MapFallbackToPage("/_Host");

app.Run();
