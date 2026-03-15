using BlankApp1.Events;
using Microsoft.AspNetCore.SignalR.Client;
using Prism.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlankApp1.Services
{
    public class VideoService : IVideoService
    {
        private readonly ISignalRService _signalRService;
        private readonly IEventAggregator _eventAggregator;

        public VideoService(ISignalRService signalRService, IEventAggregator eventAggregator)
        {
            _signalRService = signalRService;
            _eventAggregator = eventAggregator;

            // SRP: VideoService handles video-specific hub events
            _signalRService.On<byte[]>("ReceiveVideoChunk", (chunk) =>
            {
                _eventAggregator.GetEvent<VideoChunkReceivedEvent>().Publish(chunk);
            });
        }

        public async Task SendVideoChunkAsync(byte[] chunk, CancellationToken cancellationToken = default)
        {
            if (_signalRService.State == HubConnectionState.Connected)
            {
                try
                {
                    // Using SendAsync for fire-and-forget streaming performance
                    await _signalRService.SendAsync("SendVideoChunk", chunk, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when streaming is stopped
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error sending video chunk: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Cannot send video chunk: SignalR not connected.");
            }
        }
    }
}
