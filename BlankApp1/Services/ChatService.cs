using BlankApp1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public class ChatService : IChatService
    {
        public event Action<ChatMessage> MessageReceived;

        public void SendMessage(string user, string message)
        {
            throw new NotImplementedException();
        }
    }
}
