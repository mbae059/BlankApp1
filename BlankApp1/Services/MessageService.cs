using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    internal class MessageService : IMessageService
    {
        public string GetMessage() => "Hello from MessageService!";
    }
}
