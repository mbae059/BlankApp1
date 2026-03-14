using BlankApp1.Models.DTO;
using BlankApp1.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace BlankApp1.ViewModels
{
    public class MainWindowChatInputViewModel : BindableBase
    {
        private readonly IChatService _chatService;
        private string _currentInput;
        public string CurrentInput
        {
            get { return _currentInput; }
            set { SetProperty(ref _currentInput, value); }
        }

        public DelegateCommand SendMessageCommand { get; }

        public MainWindowChatInputViewModel(IChatService chatService)
        {
            _chatService = chatService;
            CurrentInput = string.Empty;

            

            SendMessageCommand = new DelegateCommand(async () =>
            {
                if (string.IsNullOrWhiteSpace(CurrentInput)) return; //No Input
                if(_chatService.State != Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected) return; //Not Connected

                MessagePayLoad messagePayLoad = new MessagePayLoad
                {
                    User = "User1", // Replace with actual user identifier
                    Message = CurrentInput,
                };

                await _chatService.SendMessageAsync(messagePayLoad); // Replace "User1" with actual user identifier
                CurrentInput = string.Empty; // Clear input after sending
            });
        }
    }
}
