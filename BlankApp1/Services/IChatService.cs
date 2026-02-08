using BlankApp1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public interface IChatService
    {
        void SendMessage(string user, string message);
        event Action<ChatMessage> MessageReceived; // This "fires" when a friend types
    }
}
