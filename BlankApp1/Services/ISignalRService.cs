using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public enum HubType
    {
        Chat,
        Stream
    }

    public interface ISignalRService
    {
        HubConnectionState GetState(HubType hubType);
        Task ConnectAsync(HubType hubType);
        Task DisconnectAsync(HubType hubType);
        Task SendAsync(HubType hubType, string methodName, object arg1, CancellationToken cancellationToken = default);
        Task InvokeAsync(HubType hubType, string methodName, object arg1, CancellationToken cancellationToken = default);
        IDisposable On<T>(HubType hubType, string methodName, Action<T> handler);
    }
}
