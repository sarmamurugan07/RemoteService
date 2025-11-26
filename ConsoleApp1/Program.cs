//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.DependencyInjection;
//using System.Threading.Tasks;

//var builder = WebApplication.CreateBuilder(args);

//// Enable CORS for Vue frontend
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowLocalhost", policy =>
//    {
//        policy.WithOrigins("http://localhost:8081", "https://localhost:8081")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

//// Add SignalR
//builder.Services.AddSignalR();

//var app = builder.Build();

//// Enable routing
//app.UseRouting();

//// Enable CORS
//app.UseCors("AllowLocalhost");

//// Map SignalR hub
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapHub<RemoteHub>("/remote");
//});

//// Run app
//app.Run("http://localhost:5000");

//// ----------------------
//// SignalR Hub
//// ----------------------
//public class RemoteHub : Hub
//{
//    // Receive control events from one client and broadcast to others
//    public async Task SendControl(string message)
//    {
//        await Clients.Others.SendAsync("ReceiveControl", message);
//    }
//}
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.DependencyInjection;
//using System.Threading.Tasks;

//var builder = WebApplication.CreateBuilder(args);

//// Enable CORS for Vue frontend


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowLocalhost", policy =>
//    {
//        policy.WithOrigins("http://localhost:8080", "http://localhost:8081")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

//// Add SignalR
//builder.Services.AddSignalR();

//var app = builder.Build();

//app.UseCors("AllowLocalhost");

//app.MapHub<RemoteHub>("/remote"); // SignalR Hub

//app.Run("http://localhost:5000");

//// ----------------------
//// SignalR Hub
//// ----------------------
//public class RemoteHub : Hub
//{
//    // Relay messages between broadcaster and viewer
//    public async Task SendControl(string message)
//    {
//        await Clients.Others.SendAsync("ReceiveControl", message);
//    }
//}

//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.DependencyInjection;
//using System.Threading.Tasks;

//var builder = WebApplication.CreateBuilder(args);

//// Add SignalR
//builder.Services.AddSignalR();

//// Enable CORS for Vue frontend
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowLocalhost", policy =>
//    {
//        policy.WithOrigins("http://localhost:8080", "http://localhost:5000", "https://localhost:8081")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

//var app = builder.Build();

//// Enable CORS before routing
//app.UseCors("AllowLocalhost");

//app.MapHub<RemoteHub>("/remote");

//app.Run("http://localhost:5000");

//// ----------------------
//// SignalR Hub
//// ----------------------
//public class RemoteHub : Hub
//{
//    // Relay messages between broadcaster and viewer
//    public async Task SendControl(string message)
//    {
//        await Clients.Others.SendAsync("ReceiveControl", message);
//    }
//}
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add SignalR
builder.Services.AddSignalR();

// Enable CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("https://stupendous-fairy-dce89e.netlify.app", "https://localhost:8081")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Enable CORS
app.UseCors("AllowLocalhost");

// Map SignalR Hub
app.MapHub<RemoteHub>("/remote");

app.Run("http://localhost:5000");

// ----------------------
// SignalR Hub
// ----------------------
public class RemoteHub : Hub
{
    // Broadcast control and signaling messages to other clients
    public async Task SendControl(string message)
    {
        await Clients.Others.SendAsync("ReceiveControl", message);
    }
}
