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

        private readonly IRegionManager _regionManager;

        public DelegateCommand<string> NavigateCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(ExcuteNavigate);
        }

        private void ExcuteNavigate(string navigatePath)
        {
            if (string.IsNullOrEmpty(navigatePath)) return;
            _regionManager.RequestNavigate("ContentRegion", navigatePath);
        }
    }
}
