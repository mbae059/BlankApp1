using BlankApp1.Events;
using BlankApp1.Models;
using BlankApp1.Models.DTO;
using Microsoft.AspNetCore.SignalR.Client;
using Prism.Events;
using System;
using System.Data.Common;
using System.Threading.Tasks;


namespace BlankApp1.Services
{
    public class ChatService : IChatService
    {
        private HubConnection _hubConnection;
        private readonly IEventAggregator _eventAggregator;


        public ChatService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            // TODO: Replace 'localhost' with your server's public IP address or domain name
            // when your friends are connecting over the internet.
            // Example: "http://12.34.56.78:5237/chathub"
            string serverUrl = "http://182.216.19.37:5237/chathub";
            string testUrl = "http://localhost:5237/chathub";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(serverUrl)
                .Build();

            _hubConnection.On<MessagePayLoad>("ReceiveMessage", (messagePayLoad) =>
            {
                // When a message is received, publish it to the UI
                _eventAggregator.GetEvent<MessageReceivedEvent>().Publish(messagePayLoad);
            });
        }

        public HubConnectionState State => _hubConnection.State;

        public async Task ConnectAsync()
        {
            while (_hubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _hubConnection.StartAsync();
                    Console.WriteLine("Connected to Hub!");
                }
                catch
                {
                    // Wait 2 seconds before trying again if the server isn't ready
                    await Task.Delay(2000);
                    Console.WriteLine("Server not ready, retrying...");
                }
            }
        }

        public async Task DisconnetAsync() => await _hubConnection.StopAsync();

        public async Task SendMessageAsync(MessagePayLoad messagePayLoad)
        {
            await _hubConnection.InvokeAsync("SendMessage", messagePayLoad);
        }
    }
}
