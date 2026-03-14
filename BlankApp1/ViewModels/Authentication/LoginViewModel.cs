using BlankApp1.Models.User;
using BlankApp1.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace BlankApp1.ViewModels.Authentication
{
    public class LoginViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private IProfileService _profileService;
        private string _username;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public DelegateCommand LoginCommand { get; }

        public LoginViewModel(IRegionManager regionManager, IProfileService profileService)
        {
            _regionManager = regionManager;
            _profileService = profileService;

            LoginCommand = new DelegateCommand(ExecuteLogin)
                .ObservesProperty(() => CanExcuteLogin);
        }

        public bool CanExcuteLogin => !string.IsNullOrWhiteSpace(Username);

        private void ExecuteLogin()
        {
            _profileService.Login(Username);
            _regionManager.RequestNavigate("MainRegion", "MainContentView");
        }
    }
}
