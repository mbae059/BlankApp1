using BlankApp1.Events;
using BlankApp1.Models.DTO;
using Microsoft.Extensions.Logging;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public class ChatService : IChatService
    {
        private readonly ISignalRService _signalRService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<ChatService> _logger;

        public ChatService(ISignalRService signalRService, IEventAggregator eventAggregator, ILogger<ChatService> logger)
        {
            _signalRService = signalRService;
            _eventAggregator = eventAggregator;
            _logger = logger;

            // SRP: ChatService handles chat-specific hub events
            _signalRService.On<MessagePayLoad>(HubType.Chat, "ReceiveMessage", (messagePayLoad) =>
            {
                // Convert UTC timestamp from server to local time for display
                messagePayLoad.Timestamp = messagePayLoad.Timestamp.ToLocalTime();
                
                // When a message is received, publish it to the UI
                _eventAggregator.GetEvent<MessageReceivedEvent>().Publish(messagePayLoad);
            });
        }

        public async Task SendMessageAsync(MessagePayLoad messagePayLoad)
        {
            try
            {
                if (_signalRService.GetState(HubType.Chat) != Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
                {
                    await _signalRService.ConnectAsync(HubType.Chat);
                }
                await _signalRService.SendAsync(HubType.Chat, "SendMessage", messagePayLoad);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message: {ex.Message}");
            }
        }
    }
}
