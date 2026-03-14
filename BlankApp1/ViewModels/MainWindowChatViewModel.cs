using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace BlankApp1.ViewModels
{
    public class MainWindowChatViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        public MainWindowChatViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            _regionManager.RegisterViewWithRegion("ChatHistoryRegion", "MainWindowChatHistory");
            _regionManager.RegisterViewWithRegion("ChatInputRegion", "MainWindowChatInput");
        }
    }
}
