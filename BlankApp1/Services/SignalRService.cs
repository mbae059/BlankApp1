using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly HubConnection _hubConnection;
        private readonly ILogger<SignalRService> _logger;

        public SignalRService(ILogger<SignalRService> logger)
        {
            _logger = logger;
            string serverUrl = "http://182.216.19.37:5237/chathub";
            
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(serverUrl)
                .WithAutomaticReconnect()
                .Build();

            // Increase the maximum message size to 10MB to accommodate video chunks
            _hubConnection.Reconnecting += (error) =>
            {
                _logger.LogError("Connection lost. Attempting to reconnect...");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                _logger.LogInformation($"Connection Reconnected! {connectionId}");
                return Task.CompletedTask;
            };
        }

        public HubConnectionState State => _hubConnection.State;

        public async Task ConnectAsync()
        {
            while (_hubConnection.State != HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.StartAsync();
                    _logger.LogInformation("Successfully connected to Hub!");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to connect to Hub: {ex.Message}. Retrying in 2 seconds...");
                    await Task.Delay(2000);
                }
            }
        }

        public async Task DisconnectAsync() => await _hubConnection.StopAsync();

        public async Task SendAsync(string methodName, object arg1, CancellationToken cancellationToken = default)
        {
            try
            {
                await _hubConnection.SendAsync(methodName, arg1, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"SendAsync for {methodName} was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendAsync for {methodName}: {ex.Message}");
                throw;
            }
        }

        public async Task InvokeAsync(string methodName, object arg1, CancellationToken cancellationToken = default)
        {
            try
            {
                await _hubConnection.InvokeAsync(methodName, arg1, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"InvokeAsync for {methodName} was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in InvokeAsync for {methodName}: {ex.Message}");
                throw;
            }
        }

        public IDisposable On<T>(string methodName, Action<T> handler)
        {
            return _hubConnection.On<T>(methodName, handler);
        }
    }
}
