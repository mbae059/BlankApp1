using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Events;
using BlankApp1.Events;
using Prism.Navigation;

namespace BlankApp1.ViewModels
{
    public class MainWindowDMListViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand StartStreamingCommand { get; }
        public DelegateCommand StopStreamingCommand { get; }

        public MainWindowDMListViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            StartStreamingCommand = new DelegateCommand(OnStartStreaming);
            StopStreamingCommand = new DelegateCommand(OnStopStreaming);
        }

        private void OnStartStreaming()
        {
            var parameters = new NavigationParameters { { "isStreamer", true } };
            _regionManager.RequestNavigate("StreamingRegion", "StreamingView", parameters);
        }

        private void OnStopStreaming()
        {
            if (_regionManager.Regions.ContainsRegionWithName("StreamingRegion"))
            {
                _eventAggregator.GetEvent<StopStreamingEvent>().Publish();
                _regionManager.Regions["StreamingRegion"].RemoveAll();
            }
        }
    }
}
