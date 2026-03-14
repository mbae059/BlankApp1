using BlankApp1.Events;
using BlankApp1.Models;
using BlankApp1.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System.Collections.ObjectModel;
namespace BlankApp1.ViewModels
{
    public class MainWindowServerListViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IProfileService _profileService;
        private readonly IRegionManager _regionManager;
        public ObservableCollection<ServerModel> Servers { get; set; }

        private ServerModel _selectedServer;
        public ServerModel SelectedServer
        {
            get => _selectedServer;
            set
            {
                if (SetProperty(ref _selectedServer, value) && value != null)
                {
                    // Publish the selected server name when a server is selected
                    _eventAggregator.GetEvent<ServerSelectedEvent>().Publish(value.Name);
                }
            }
        }

        public DelegateCommand LogoutCommand { get; }

        public MainWindowServerListViewModel(IEventAggregator eventAggregator, IProfileService profileService, IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _profileService = profileService;
            _regionManager = regionManager;

            Servers = new ObservableCollection<ServerModel>
            {
                new ServerModel { Name = "Gaming", IconColor = "#5865F2" }, // Discord Blue
                new ServerModel { Name = "Programming", IconColor = "#3BA55C" }, // Green
                new ServerModel { Name = "Music", IconColor = "#ED4245" }, // Red
                new ServerModel { Name = "Art", IconColor = "#FEE75C" } // Yellow
            };

            LogoutCommand = new DelegateCommand(ExecuteLogout);
        }

        private void ExecuteLogout()
        {
            _profileService.Logout();
            _regionManager.RequestNavigate("MainRegion", "LoginView");
        }
    }
}
