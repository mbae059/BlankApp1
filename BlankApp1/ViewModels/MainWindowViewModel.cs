using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace BlankApp1.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private string _title = "Hello Prism 9!";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            _regionManager.RegisterViewWithRegion("ServerListRegion", "MainWindowServerList");
            _regionManager.RegisterViewWithRegion("DMListRegion", "MainWindowDMList");
            _regionManager.RegisterViewWithRegion("ChatRegion", "MainWindowChat");
        }

    }
}
