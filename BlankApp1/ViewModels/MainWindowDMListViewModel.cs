using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Events;
using BlankApp1.Events;

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
            _regionManager.RequestNavigate("StreamingRegion", "StreamingView");
        }

        private void OnStopStreaming()
        {
            if (_regionManager.Regions.ContainsRegionWithName("StreamingRegion"))
            {
                _eventAggregator.GetEvent<StopStreamingEvent>().Publish("stop");
                _regionManager.Regions["StreamingRegion"].RemoveAll();
            }
        }
    }
}
