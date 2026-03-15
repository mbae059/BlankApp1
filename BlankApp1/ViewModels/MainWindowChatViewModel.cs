using BlankApp1.Events;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;
using System.Linq;

namespace BlankApp1.ViewModels
{
    public class MainWindowChatViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        public MainWindowChatViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            _regionManager.RegisterViewWithRegion("ChatHistoryRegion", "MainWindowChatHistory");
            _regionManager.RegisterViewWithRegion("ChatInputRegion", "MainWindowChatInput");

            // Automatically open the streaming view if a video chunk arrives
            _eventAggregator.GetEvent<VideoChunkReceivedEvent>().Subscribe(OnVideoChunkReceived, ThreadOption.UIThread);
        }

        private void OnVideoChunkReceived(byte[] chunk)
        {
            // If the streaming region is empty, navigate to the StreamingView (as a receiver)
            var region = _regionManager.Regions["StreamingRegion"];
            if (region != null && !region.Views.Any())
            {
                // Navigation parameter is false because this is a receiver, not the streamer
                var parameters = new NavigationParameters { { "isStreamer", false } };
                _regionManager.RequestNavigate("StreamingRegion", "StreamingView", parameters);
            }
        }
    }
}
