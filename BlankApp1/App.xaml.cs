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
            containerRegistry.RegisterForNavigation<MainWindowServerList>("MainWindowServerList");
            containerRegistry.RegisterForNavigation<MainWindowDMList>("MainWindowDMList");
            containerRegistry.RegisterForNavigation<MainWindowChat>("MainWindowChat");
            containerRegistry.RegisterForNavigation<MainWindowChatHistory>("MainWindowChatHistory");
            containerRegistry.RegisterForNavigation<MainWindowChatInput>("MainWindowChatInput");

            containerRegistry.RegisterSingleton<IChatService, ChatService>();

        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            // Connect to the chat service when the application starts
            var chatService = Container.Resolve<IChatService>();
            chatService.ConnectAsync();
        }
    }
}
