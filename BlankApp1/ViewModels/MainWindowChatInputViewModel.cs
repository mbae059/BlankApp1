using BlankApp1.Models.DTO;
using BlankApp1.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace BlankApp1.ViewModels
{
    public class MainWindowChatInputViewModel : BindableBase
    {
        private readonly IChatService _chatService;
        private readonly IProfileService _profileService;
        private string _currentInput;
        public string CurrentInput
        {
            get { return _currentInput; }
            set { SetProperty(ref _currentInput, value); }
        }

        public DelegateCommand SendMessageCommand { get; }

        public MainWindowChatInputViewModel(IChatService chatService, IProfileService profileService)
        {
            _chatService = chatService;
            _profileService = profileService;
            CurrentInput = string.Empty;

            

            SendMessageCommand = new DelegateCommand(async () =>
            {
                if (string.IsNullOrWhiteSpace(CurrentInput)) return; //No Input
                
                MessagePayLoad messagePayLoad = new MessagePayLoad
                {
                    User = _profileService.profile.UserName,
                    Message = CurrentInput,
                };

                await _chatService.SendMessageAsync(messagePayLoad); 
                CurrentInput = string.Empty; // Clear input after sending
            });
        }
    }
}
