using BlankApp1.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5237); // This binds to 0.0.0.0:5237
});

builder.Services.AddSignalR(); // 1. Add SignalR services
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.AllowAnyHeader()
//              .AllowAnyMethod()
//              .SetIsOriginAllowed(_ => true) // Allow any origin
//              .AllowCredentials();
//    });
//});


var app = builder.Build();
//app.UseCors("AllowAll");
app.MapGet("/", () => "Server is Running! SignalR is waiting at /chathub");
app.MapHub<ChatHub>("/chathub"); // 2. Create the "Portal" address

app.Run();