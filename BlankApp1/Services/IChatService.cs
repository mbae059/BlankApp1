using BlankApp1.Models;
using BlankApp1.Models.DTO;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public interface IChatService
    {
        Task ConnectAsync();
        Task DisconnetAsync();
        Task SendMessageAsync(MessagePayLoad messagePayLoad);
        HubConnectionState State { get; }
    }
}
