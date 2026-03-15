using BlankApp1.Services;
using BlankApp1.Views;
using BlankApp1.Views.Authentication;
using DryIoc;
using Microsoft.Extensions.Logging;
using Prism.Container.DryIoc;
using Prism.Ioc;
using System.Windows;

namespace BlankApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<ShellWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterLogger(containerRegistry);

            containerRegistry.RegisterForNavigation<LoginView>("LoginView");

            containerRegistry.RegisterForNavigation<MainContentView>("MainContentView");

            containerRegistry.RegisterForNavigation<MainWindowServerListView>("MainWindowServerList");
            containerRegistry.RegisterForNavigation<MainWindowDMListView>("MainWindowDMList");
            containerRegistry.RegisterForNavigation<MainWindowChatView>("MainWindowChat");
            containerRegistry.RegisterForNavigation<MainWindowChatHistoryView>("MainWindowChatHistory");
            containerRegistry.RegisterForNavigation<MainWindowChatInputView>("MainWindowChatInput");
            containerRegistry.RegisterForNavigation<StreamingView>("StreamingView");

            containerRegistry.RegisterSingleton<ISignalRService, SignalRService>();
            containerRegistry.RegisterSingleton<IChatService, ChatService>();
            containerRegistry.RegisterSingleton<IVideoService, VideoService>();
            containerRegistry.RegisterSingleton<IProfileService, ProfileService>();
        }

        protected void RegisterLogger(IContainerRegistry containerRegistry)
        {
            // 1. Create the Factory (This is what actually creates the loggers)
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddDebug(); // This sends logs to the Visual Studio Output window
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // 2. Register the Factory as a singleton
            containerRegistry.RegisterInstance<ILoggerFactory>(loggerFactory);

            // 3. Register the generic ILogger<T> type
            // This tells Prism: "If someone asks for ILogger<AnyClass>, give them a Logger<AnyClass>"
            var container = containerRegistry.GetContainer();
            container.Register(typeof(ILogger<>), typeof(Logger<>), setup: Setup.With(condition: r => r.Parent != null));
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            // Connect to both SignalR hubs when the application starts
            var signalRService = Container.Resolve<ISignalRService>();
            signalRService.ConnectAsync(HubType.Chat);
            signalRService.ConnectAsync(HubType.Stream);
        }
    }
}
