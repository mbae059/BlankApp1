using BlankApp1.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5237); // This binds to 0.0.0.0:5237
});

builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
    options.EnableDetailedErrors = true;
}).AddMessagePackProtocol(); // Enable binary protocol

var app = builder.Build();

app.MapGet("/", () => "Server is Running! SignalR is waiting at /chathub and /streamhub");
app.MapHub<ChatHub>("/chathub");
app.MapHub<StreamHub>("/streamhub");

app.Run();