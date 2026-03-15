using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public interface ISignalRService
    {
        HubConnectionState State { get; }
        Task ConnectAsync();
        Task DisconnectAsync();
        Task SendAsync(string methodName, object arg1, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, object arg1, CancellationToken cancellationToken = default);
        IDisposable On<T>(string methodName, Action<T> handler);
    }
}
