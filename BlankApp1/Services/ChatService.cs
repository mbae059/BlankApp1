using BlankApp1.Events;
using BlankApp1.Models.DTO;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Prism.Events;
using System;
using System.Threading.Tasks;


namespace BlankApp1.Services
{
    public class ChatService : IChatService
    {
        private HubConnection _hubConnection;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IEventAggregator eventAggregator, ILogger<ChatService> logger)
        {
            _eventAggregator = eventAggregator;
            _logger = logger;
            // TODO: Replace 'localhost' with your server's public IP address or domain name
            // when your friends are connecting over the internet.
            // Example: "http://12.34.56.78:5237/chathub"
            string serverUrl = "http://182.216.19.37:5237/chathub";
            string testUrl = "http://localhost:5237/chathub";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(serverUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.Reconnecting += (error) =>
            {
                _logger.LogError("Connection lost. Attempting to reconnect...");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                _logger.LogInformation($"Connection Reconnected! {connectionId}");
                return Task.CompletedTask;
            };


            _hubConnection.On<MessagePayLoad>("ReceiveMessage", (messagePayLoad) =>
            {
                // Convert UTC timestamp from server to local time for display
                messagePayLoad.Timestamp = messagePayLoad.Timestamp.ToLocalTime();
                
                // When a message is received, publish it to the UI
                _eventAggregator.GetEvent<MessageReceivedEvent>().Publish(messagePayLoad);
            });
        }

        public HubConnectionState State => _hubConnection.State;

        public async Task ConnectAsync()
        {
            // Continue trying until the hub connection is successfully established.
            // Using HubConnectionState.Connected as the condition ensures we don't
            // stop trying prematurely (e.g., if it's stuck in 'Connecting' or 'Reconnecting').
            while (_hubConnection.State != HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.StartAsync();
                    _logger.LogInformation("Successfully connected to Hub!");
                }
                catch (Exception ex)
                {
                    // Log the specific error to help with troubleshooting
                    _logger.LogWarning($"Failed to connect to Hub: {ex.Message}. Retrying in 2 seconds...");
                    // Wait 2 seconds before trying again if the server isn't ready
                    await Task.Delay(2000);
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
