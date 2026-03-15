using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace BlankApp1.ViewModels
{
    public class ShellWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private string _title = "Blank";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ShellWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            _regionManager.RegisterViewWithRegion("MainRegion", "LoginView");
        }

    }
}
