using BlankApp1.Events;
using BlankApp1.Models;
using BlankApp1.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp1.ViewModels
{
    public class MainWindowChatViewModel : BindableBase
    {
        private readonly IChatService _chatService;
        public ObservableCollection<ChatMessage> ChatHistory { get; set; } = new();

        public string NewMessageText { get; set; }
        public DelegateCommand SendCommand { get; }
        public MainWindowChatViewModel(IChatService chatService)
        {
            _chatService = chatService;

            _chatService.MessageReceived += OnMessageReceived;

            SendCommand = new DelegateCommand(() =>
            {
                _chatService.SendMessage("Me", NewMessageText);
                NewMessageText = string.Empty; // Clear input
                RaisePropertyChanged(nameof(NewMessageText));
            });
        }
        private void OnMessageReceived(ChatMessage msg)
        {
            // WPF requires UI updates to happen on the Main Thread
            App.Current.Dispatcher.Invoke(() => ChatHistory.Add(msg));
        }

    }
}
