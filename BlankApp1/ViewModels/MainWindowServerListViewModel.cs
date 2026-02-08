using BlankApp1.Services;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp1.ViewModels
{
    public class MainWindowServerListViewModel : BindableBase
    {
        private string _serviceMessage;
        public string ServiceMessage
        {
            get { return _serviceMessage; }
            set { SetProperty(ref _serviceMessage, value); }
        }

        public MainWindowServerListViewModel(IMessageService messageService)
        {
            ServiceMessage = messageService.GetMessage();
        }
    }
}
