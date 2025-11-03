using AnomalyCounter.Anomalies;
using AnomalyCounter.Components;
using AnomalyCounter.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// automatically register all IAnomalyCounter implementations
var anomalyCounterType = typeof(IAnomalyCounter);
var assembly = Assembly.GetExecutingAssembly();

var implementations = assembly.GetTypes()
    .Where(t => anomalyCounterType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

foreach (var impl in implementations)
{
    builder.Services.AddSingleton(anomalyCounterType, impl);
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
