using Prism.Events;

namespace BlankApp1.Events
{
    public class VideoChunkReceivedEvent : PubSubEvent<byte[]>
    {
    }
}
