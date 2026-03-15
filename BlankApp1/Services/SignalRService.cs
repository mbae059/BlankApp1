using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly Dictionary<HubType, HubConnection> _connections = new Dictionary<HubType, HubConnection>();
        private readonly ILogger<SignalRService> _logger;

        public SignalRService(ILogger<SignalRService> logger)
        {
            _logger = logger;
            string baseUrl = "http://182.216.19.37:5237";
            
            _connections[HubType.Chat] = CreateConnection($"{baseUrl}/chathub", "Chat");
            _connections[HubType.Stream] = CreateConnection($"{baseUrl}/streamhub", "Stream");
        }

        private HubConnection CreateConnection(string url, string name)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(url)
                .AddMessagePackProtocol() // Use binary protocol for better performance
                .WithAutomaticReconnect()
                .Build();

            connection.Reconnecting += (error) =>
            {
                _logger.LogError($"{name} connection lost. Attempting to reconnect...");
                return Task.CompletedTask;
            };

            connection.Reconnected += (connectionId) =>
            {
                _logger.LogInformation($"{name} connection reconnected! {connectionId}");
                return Task.CompletedTask;
            };

            return connection;
        }

        public HubConnectionState GetState(HubType hubType) => _connections[hubType].State;

        public async Task ConnectAsync(HubType hubType)
        {
            var connection = _connections[hubType];
            while (connection.State != HubConnectionState.Connected)
            {
                try
                {
                    await connection.StartAsync();
                    _logger.LogInformation($"Successfully connected to {hubType} Hub!");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to connect to {hubType} Hub: {ex.Message}. Retrying in 2 seconds...");
                    await Task.Delay(2000);
                }
            }
        }

        public async Task DisconnectAsync(HubType hubType) => await _connections[hubType].StopAsync();

        public async Task SendAsync(HubType hubType, string methodName, object arg1, CancellationToken cancellationToken = default)
        {
            try
            {
                await _connections[hubType].SendAsync(methodName, arg1, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"SendAsync for {methodName} on {hubType} was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendAsync for {methodName} on {hubType}: {ex.Message}");
                throw;
            }
        }

        public async Task InvokeAsync(HubType hubType, string methodName, object arg1, CancellationToken cancellationToken = default)
        {
            try
            {
                await _connections[hubType].InvokeAsync(methodName, arg1, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"InvokeAsync for {methodName} on {hubType} was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in InvokeAsync for {methodName} on {hubType}: {ex.Message}");
                throw;
            }
        }

        public IDisposable On<T>(HubType hubType, string methodName, Action<T> handler)
        {
            return _connections[hubType].On<T>(methodName, handler);
        }
    }
}
