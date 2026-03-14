using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RemoteControl.Server;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

static string GetLocalIPv4()
{
    return Dns.GetHostEntry(Dns.GetHostName())
        .AddressList
        .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork &&
                             !ip.ToString().StartsWith("169.254."))?.ToString() ?? "NO_IP";
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

string lanIp = GetLocalIPv4();
const int Port = 64751;

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(Port, listenOptions =>
    {
        listenOptions.UseHttps("C:\\lan-cert.pfx", "test");
    });
});

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowFrontend");
app.MapHub<RemoteHub>("/remote");

Console.WriteLine("===============================================");
Console.WriteLine($" LAN IP  : {lanIp}");
Console.WriteLine($" URL     : https://{lanIp}:{Port}");
Console.WriteLine("===============================================");

app.Run();
