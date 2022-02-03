using HomeAutomation.BlazorApp.Server.BackgroundServices;
using HomeAutomation.BlazorApp.Server.Hubs;
using HomeAutomation.Core.Contracts.Persistence;
using HomeAutomation.Core.Contracts.Services;
using HomeAutomation.Persistence.InMemory;
using HomeAutomation.Services.Mqtt;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IMqttService, MqttService>();
builder.Services.AddSingleton<IDeviceRepository, InMemoryDeviceRepository>();

builder.Services.AddHostedService<HomeAutomationMonitor>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});


var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapHub<HomeAutomationHub>("/home-automation");
app.MapFallbackToFile("index.html");

app.Run();
