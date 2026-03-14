using BlankApp1.Server.DTO;
using Microsoft.AspNetCore.SignalR;

namespace BlankApp1.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(MessagePayLoad messagePayLoad)
        {
            messagePayLoad.Timestamp = DateTime.UtcNow; // Add timestamp on the server side
            // This takes a message from one friend and blasts it to everyone else
            await Clients.All.SendAsync("ReceiveMessage", messagePayLoad);
        }

        public async Task SendVideoChunk(byte[] chunk)
        {
            // Broadcasts the video byte stream to others
            await Clients.Others.SendAsync("ReceiveVideoChunk", chunk);
        }
    }
}
