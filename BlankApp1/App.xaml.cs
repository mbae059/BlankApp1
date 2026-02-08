using System.Windows;
using BlankApp1.Services;
using BlankApp1.Views;
using Prism.Ioc;

namespace BlankApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainWindowDMList>("PageA");
            containerRegistry.RegisterForNavigation<MainWindowServerList>("PageB");

            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
        }
    }
}
