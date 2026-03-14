using BlankApp1.Events;
using BlankApp1.Models.DTO;
using BlankApp1.Services;
using Microsoft.Extensions.Logging;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System.Collections.ObjectModel;

namespace BlankApp1.ViewModels
{
    public class MainWindowChatHistoryViewModel : BindableBase, IDestructible
    {
        public ObservableCollection<MessagePayLoad> ChatMessages { get; } = new();
        private readonly IEventAggregator _eventAggregator;
        private readonly SubscriptionToken _messageReceivedToken;
        private readonly ILogger<MainWindowChatHistoryViewModel> _logger;

        public MainWindowChatHistoryViewModel(IEventAggregator eventAggregator, ILogger<MainWindowChatHistoryViewModel> logger)
        {
            _eventAggregator = eventAggregator;
            _logger = logger;

            // Using keepSubscriberReferenceAlive: true ensures that the subscription 
            // is not garbage collected while the application is running.
            // We store the token to unsubscribe when the viewmodel is destroyed.
            _messageReceivedToken = _eventAggregator.GetEvent<MessageReceivedEvent>().Subscribe(OnMessageReceived, ThreadOption.UIThread, true);
        }

        private void OnMessageReceived(MessagePayLoad payload)
        {
            ChatMessages.Add(payload);
            _logger.LogInformation($"Received message from {payload.User}: {payload.Message}");
        }

        public void Destroy()
        {
            // Unsubscribe when the ViewModel is destroyed to prevent memory leaks
            if (_messageReceivedToken != null)
            {
                _eventAggregator.GetEvent<MessageReceivedEvent>().Unsubscribe(_messageReceivedToken);
                _logger.LogInformation("Unsubscribed from MessageReceivedEvent");
            }
        }
    }
}
