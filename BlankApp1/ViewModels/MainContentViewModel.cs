using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlankApp1.ViewModels
{
	public class MainContentViewModel : BindableBase
	{
        private readonly IRegionManager _regionManager;
        public MainContentViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            _regionManager.RegisterViewWithRegion("ServerListRegion", "MainWindowServerListView");
            _regionManager.RegisterViewWithRegion("DMListRegion", "MainWindowDMListView");
            _regionManager.RegisterViewWithRegion("ChatRegion", "MainWindowChatView");

        }
	}
}
