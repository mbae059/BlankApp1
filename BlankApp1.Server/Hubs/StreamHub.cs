using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace BlankApp1.Server.Hubs
{
    public class StreamHub : Hub
    {
        // Track total bytes per connection
        private static readonly ConcurrentDictionary<string, long> _byteCounters = new ConcurrentDictionary<string, long>();
        
        // Track time and bytes for throughput calculation (LastTime, LastTotalBytes)
        private static readonly ConcurrentDictionary<string, (DateTime LastTime, long LastBytes)> _throughputStats = new ConcurrentDictionary<string, (DateTime, long)>();

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[StreamHub] Client Joined: {Context.ConnectionId}");
            _byteCounters[Context.ConnectionId] = 0;
            _throughputStats[Context.ConnectionId] = (DateTime.UtcNow, 0);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _byteCounters.TryRemove(Context.ConnectionId, out long totalBytes);
            _throughputStats.TryRemove(Context.ConnectionId, out _);
            Console.WriteLine($"[StreamHub] Client Left: {Context.ConnectionId}. Total Data Relayed: {totalBytes / 1024 / 1024} MB");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendVideoChunk(byte[] chunk)
        {
            if (chunk == null) return;

            string cid = Context.ConnectionId;
            long totalBytes = _byteCounters.AddOrUpdate(cid, chunk.Length, (_, old) => old + chunk.Length);

            // Throughput calculation
            var now = DateTime.UtcNow;
            if (_throughputStats.TryGetValue(cid, out var stats))
            {
                double elapsed = (now - stats.LastTime).TotalSeconds;
                if (elapsed >= 1.0) // Log once per second
                {
                    if (_throughputStats.TryUpdate(cid, (now, totalBytes), stats))
                    {
                        long diff = totalBytes - stats.LastBytes;
                        double kbps = diff / 1024.0 / elapsed;
                        double mbps = (diff * 8.0) / (1024.0 * 1024.0) / elapsed;

                        Console.WriteLine($"[Stream] {cid} | {kbps:F2} KB/s | {mbps:F2} Mbps | Total: {totalBytes / 1024 / 1024} MB");
                    }
                }
            }
            else
            {
                _throughputStats.TryAdd(cid, (now, totalBytes));
            }

            // Relay raw bytes to others
            await Clients.Others.SendAsync("ReceiveVideoChunk", chunk);
        }

        public async Task StopStreaming()
        {
            Console.WriteLine($"[Stream] Stop signal received from {Context.ConnectionId}");
            await Clients.Others.SendAsync("ReceiveStopStreaming", Context.ConnectionId);
        }
    }
}
