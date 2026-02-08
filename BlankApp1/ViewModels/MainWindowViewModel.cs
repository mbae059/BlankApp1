using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace BlankApp1.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Hello Prism 9!";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion("ServerListRegion", "MainWindowServerList");
            regionManager.RegisterViewWithRegion("DMListRegion", "MainWindowDMList");
            regionManager.RegisterViewWithRegion("ChatRegion", "MainWindowChat");
        }

    }
}
