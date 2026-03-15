using BlankApp1.Events;
using BlankApp1.Services;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BlankApp1.ViewModels
{
    public class StreamingViewModel : BindableBase, INavigationAware, IDisposable, IRegionMemberLifetime
    {
        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int nIndex);
        const int SM_CXSCREEN = 0;
        const int SM_CYSCREEN = 1;

        private readonly IRegionManager _regionManager;
        private readonly IVideoService _videoService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<StreamingViewModel> _logger;

        private bool _isStreaming; // If we are sending
        private bool _isReceiver; // If we are receiving
        private bool _isActive; // If this VM session is active
        private bool _isStartingFFplay;
        private bool _isDisposed;

        private Process _ffmpegProcess;
        private Process _ffplayProcess;
        private readonly object _lockObject = new object();
        private CancellationTokenSource _captureCts;

        public DelegateCommand CloseStreamingCommand { get; }

        private SubscriptionToken _videoChunkSubscription;
        private SubscriptionToken _stopStreamingSubscription;

        public bool KeepAlive => false; // Ensure VM is removed when navigating away

        public StreamingViewModel(IRegionManager regionManager, IVideoService videoService, IEventAggregator eventAggregator, ILogger<StreamingViewModel> logger)
        {
            _regionManager = regionManager;
            _videoService = videoService;
            _eventAggregator = eventAggregator;
            _logger = logger;
            CloseStreamingCommand = new DelegateCommand(OnCloseStreaming);

            _videoChunkSubscription = _eventAggregator.GetEvent<VideoChunkReceivedEvent>().Subscribe(OnVideoChunkReceived, ThreadOption.PublisherThread);
            _stopStreamingSubscription = _eventAggregator.GetEvent<StopStreamingEvent>().Subscribe(OnStopStreamingEvent, ThreadOption.UIThread);
        }

        private void OnStopStreamingEvent() => StopCapture();

        private void OnVideoChunkReceived(byte[] chunk)
        {
            if (!_isActive || _isDisposed) return;

            bool shouldStart = false;
            lock (_lockObject)
            {
                if (!_isStartingFFplay && (_ffplayProcess == null || _ffplayProcess.HasExited))
                {
                    _isStartingFFplay = true;
                    shouldStart = true;
                }
            }

            if (shouldStart) StartFFplay();

            try
            {
                var process = _ffplayProcess;
                if (process != null && !process.HasExited)
                {
                    process.StandardInput.BaseStream.Write(chunk, 0, chunk.Length);
                    process.StandardInput.BaseStream.Flush();
                }
            }
            catch { }
        }

        private void StartFFplay()
        {
            try
            {
                if (!_isActive || _isDisposed) 
                {
                    lock (_lockObject) { _isStartingFFplay = false; }
                    return; 
                }

                string ffplayPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffplay.exe");
                if (!File.Exists(ffplayPath))
                {
                    lock (_lockObject) { _isStartingFFplay = false; }
                    return;
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = ffplayPath,
                    // Optimized for low latency and stability
                    //
                    Arguments = $"-i pipe:0 -vf \"scale=1920:1080\" -alwaysontop -window_title \"{(_isStreaming ? "My Preview" : "Received Stream")}\" -probesize 32768 -analyzeduration 0 -sync ext -fflags nobuffer -flags low_delay -framedrop -v quiet -autoexit",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                };

                _ffplayProcess = Process.Start(startInfo);
                if (_ffplayProcess != null)
                {
                    _ffplayProcess.EnableRaisingEvents = true;
                    _ffplayProcess.Exited += OnFFplayProcessExited;
                }
                _isReceiver = true;
            }
            catch 
            {
                lock (_lockObject) { _isStartingFFplay = false; }
            }
        }

        private void OnFFplayProcessExited(object sender, EventArgs e)
        {
            // If the process exited but we are still "active", it means the user closed the window manually.
            if (_isActive)
            {
                _logger.LogInformation("FFplay window closed by user. Stopping stream.");
                // Use UI thread for Prism navigation/region operations
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await StopCaptureAsync();
                    if (_regionManager.Regions.ContainsRegionWithName("StreamingRegion"))
                    {
                        _regionManager.Regions["StreamingRegion"].RemoveAll();
                    }
                });
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _isActive = true;
            bool isStreamer = navigationContext.Parameters.GetValue<bool>("isStreamer");
            if (isStreamer)
            {
                _isStreaming = true;
                StartCapture();
            }
        }

        private void StartCapture()
        {
            _captureCts?.Cancel();
            _captureCts = new CancellationTokenSource();
            Task.Run(() => CaptureWithFFmpeg(_captureCts.Token));
        }

        private async Task CaptureWithFFmpeg(CancellationToken ct)
        {
            try
            {
                string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
                if (!File.Exists(ffmpegPath)) { StopCapture(); return; }

                int physicalWidth = GetSystemMetrics(SM_CXSCREEN);
                int physicalHeight = GetSystemMetrics(SM_CYSCREEN);

                var startInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    // Improved quality with 'veryfast' preset and 'format=yuv420p', stable GOP with '-g 60'
                    Arguments = $"-f gdigrab -framerate 60 -offset_x 0 -offset_y 0 -video_size {physicalWidth}x{physicalHeight} -i desktop -vf \"scale=1920:1080:force_original_aspect_ratio=decrease,pad=1920:1080:(ow-iw)/2:(oh-ih)/2,format=yuv420p\" -c:v libx264 -preset veryfast -tune zerolatency -b:v 8M -maxrate 8M -bufsize 16M -g 60 -f mpegts pipe:1 -v quiet",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                _ffmpegProcess = Process.Start(startInfo);

                using (var stream = _ffmpegProcess.StandardOutput.BaseStream)
                {
                    byte[] buffer = new byte[65536];
                    int bytesRead;
                    while (_isActive && _isStreaming && !ct.IsCancellationRequested && 
                           (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
                    {
                        byte[] chunk = new byte[bytesRead];
                        Array.Copy(buffer, chunk, bytesRead);
                        
                        // BROADCAST TO SIGNALR (for others)
                        await _videoService.SendVideoChunkAsync(chunk, ct);
                        
                        // LOOPBACK TO LOCAL FFPLAY (for my preview)
                        OnVideoChunkReceived(chunk);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CaptureWithFFmpeg");
                StopCapture();
            }
        }

        private async void OnCloseStreaming()
        {
            await StopCaptureAsync();
            if (_regionManager.Regions.ContainsRegionWithName("StreamingRegion"))
            {
                _regionManager.Regions["StreamingRegion"].RemoveAll();
            }
        }

        private async Task StopCaptureAsync()
        {
            if (_isStreaming) await _videoService.StopStreamingAsync();

            _isActive = false;
            _isStreaming = false;
            _isReceiver = false;
            _captureCts?.Cancel();

            CleanupProcesses();
            
            lock (_lockObject) { _isStartingFFplay = false; }
        }

        private void StopCapture()
        {
            if (_isStreaming) _ = _videoService.StopStreamingAsync();

            _isActive = false;
            _isStreaming = false;
            _isReceiver = false;
            _captureCts?.Cancel();

            CleanupProcesses();
            
            lock (_lockObject) { _isStartingFFplay = false; }
        }

        private void CleanupProcesses()
        {
            try
            {
                lock (_lockObject)
                {
                    if (_ffmpegProcess != null && !_ffmpegProcess.HasExited) { _ffmpegProcess.Kill(true); _ffmpegProcess.Dispose(); }
                    if (_ffplayProcess != null && !_ffplayProcess.HasExited) { _ffplayProcess.Kill(true); _ffplayProcess.Dispose(); }
                    _ffmpegProcess = null;
                    _ffplayProcess = null;
                }
            }
            catch { }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) 
        { 
            StopCapture(); 
            UnsubscribeFromEvents(); 
        }

        public void Dispose() 
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _isActive = false;
            StopCapture(); 
            UnsubscribeFromEvents();
            _captureCts?.Dispose();
        }

        private void UnsubscribeFromEvents()
        {
            if (_videoChunkSubscription != null) { _eventAggregator.GetEvent<VideoChunkReceivedEvent>().Unsubscribe(_videoChunkSubscription); _videoChunkSubscription = null; }
            if (_stopStreamingSubscription != null) { _eventAggregator.GetEvent<StopStreamingEvent>().Unsubscribe(_stopStreamingSubscription); _stopStreamingSubscription = null; }
        }
    }
}
