using BlankApp1.Server.DTO;
using Microsoft.AspNetCore.SignalR;

namespace BlankApp1.Server.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[ChatHub] Client Connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"[ChatHub] Client Disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(MessagePayLoad messagePayLoad)
        {
            Console.WriteLine($"[Chat] {messagePayLoad.User}: {messagePayLoad.Message}");
            messagePayLoad.Timestamp = DateTime.UtcNow;
            await Clients.All.SendAsync("ReceiveMessage", messagePayLoad);
        }
    }
}
