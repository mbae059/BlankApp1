using BlankApp1.Events;
using BlankApp1.Models.DTO;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace BlankApp1.ViewModels
{
    public class MainWindowChatHistoryViewModel : BindableBase
    {
        public ObservableCollection<MessagePayLoad> ChatMessages { get; } = new();
        private readonly IEventAggregator _eventAggregator;
        public MainWindowChatHistoryViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<MessageReceivedEvent>().Subscribe(payload =>
            {
                ChatMessages.Add(payload);
                System.Diagnostics.Debug.WriteLine($"Received message from {payload.User}: {payload.Message}");
            }, ThreadOption.UIThread);
            
        }
    }
}
