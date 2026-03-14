using BlankApp1.Models.DTO;
using Prism.Events;

namespace BlankApp1.Events
{
    public class MessageReceivedEvent : PubSubEvent<MessagePayLoad>
    {
    }
}
