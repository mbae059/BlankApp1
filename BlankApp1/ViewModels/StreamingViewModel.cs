using BlankApp1.Events;
using BlankApp1.Services;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BlankApp1.ViewModels
{
    public class StreamingViewModel : BindableBase, INavigationAware, IDisposable
    {
        private readonly IRegionManager _regionManager;
        private readonly IVideoService _videoService;
        private readonly IEventAggregator _eventAggregator;
        private BitmapImage _streamSource;
        private CancellationTokenSource _captureCts;
        private bool _isStreaming;
        private readonly ILogger<StreamingViewModel> _logger;
        public BitmapImage StreamSource
        {
            get => _streamSource;
            set => SetProperty(ref _streamSource, value);
        }

        public DelegateCommand CloseStreamingCommand { get; }

        public StreamingViewModel(IRegionManager regionManager, IVideoService videoService, IEventAggregator eventAggregator, ILogger<StreamingViewModel> logger)
        {
            _regionManager = regionManager;
            _videoService = videoService;
            _eventAggregator = eventAggregator;
            _logger = logger;
            CloseStreamingCommand = new DelegateCommand(OnCloseStreaming);

            _eventAggregator.GetEvent<VideoChunkReceivedEvent>().Subscribe(OnVideoChunkReceived, ThreadOption.UIThread);
            _eventAggregator.GetEvent<StopStreamingEvent>().Subscribe(OnStopStreamingEvent, ThreadOption.UIThread);
        }

        private void OnStopStreamingEvent(string _)
        {
            StopCapture();
        }

        private void OnVideoChunkReceived(byte[] chunk)
        {
            // If we are the one streaming, we update locally in CaptureLoop
            // to avoid round-trip delay and redundant updates.
            if (_isStreaming) return;

            StreamSource = ToBitmapImage(chunk);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            StartCapture();
        }

        private void StartCapture()
        {
            if (_isStreaming) return;
            _isStreaming = true;
            _captureCts = new CancellationTokenSource();
            
            Task.Run(() => CaptureLoop(_captureCts.Token));
        }

        private async Task CaptureLoop(CancellationToken token)
        {
            // Lowered to 10 FPS to ensure we don't overwhelm the connection
            const int targetFps = 30;
            const int frameDelay = 1000 / targetFps;

            // Reduce resolution to 640x360 to keep file size down
            const int targetWidth = 640;
            const int targetHeight = 360;

            int frameCount = 0;
            while (!token.IsCancellationRequested)
            {
                var startTime = DateTime.Now;

                try
                {
                    byte[] frameData = CaptureScreen(targetWidth, targetHeight);
                    
                    // Exit if cancelled during capture
                    if (token.IsCancellationRequested) break;

                    if (frameData != null)
                    {
                        // SignalR Base64 overhead is ~33%. 24KB * 1.33 = ~32KB.
                        // Safe limit to prevent server disconnection.
                        if (frameData.Length > 24000) 
                        {
                            System.Diagnostics.Debug.WriteLine($"Frame skipped: Size {frameData.Length} exceeds safe limit.");
                            continue;
                        }

                        frameCount++;
                        // Update local preview
                        var bitmap = ToBitmapImage(frameData);
                        Application.Current.Dispatcher.Invoke(() => StreamSource = bitmap);

                        // Only send if connected
                        if (_videoService is VideoService service)
                        {
                            _logger.LogInformation($"Sending Frame #{frameCount}, Size: {frameData.Length} bytes");
                        }
                        await _videoService.SendVideoChunkAsync(frameData, token);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"CaptureLoop Iteration Error: {ex.Message}");
                    // Don't exit the loop, just log and continue to the next frame
                }

                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                var sleepTime = Math.Max(0, frameDelay - (int)elapsed);
                
                try
                {
                    await Task.Delay(sleepTime, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            
            System.Diagnostics.Debug.WriteLine("CaptureLoop Exited.");
        }

        private byte[] CaptureScreen(int width, int height)
        {
            try
            {
                // Get main screen bounds in pixels
                int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
                int screenHeight = (int)SystemParameters.PrimaryScreenHeight;

                using (Bitmap bmp = new Bitmap(screenWidth, screenHeight))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                    }

                    // Resize to target resolution
                    using (Bitmap resized = new Bitmap(bmp, new System.Drawing.Size(width, height)))
                    {
                        return ToByteArray(resized);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CaptureScreen Error: {ex.Message}");
                return null;
            }
        }

        private byte[] ToByteArray(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Use JPEG with 40% quality to ensure we stay under the 32KB default limit
                var encoder = GetEncoder(ImageFormat.Jpeg);
                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 40L);
                bitmap.Save(ms, encoder, encoderParameters);
                
                byte[] data = ms.ToArray();
                System.Diagnostics.Debug.WriteLine($"Frame Size: {data.Length} bytes");
                return data;
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private BitmapImage ToBitmapImage(byte[] array)
        {
            using (var ms = new MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }

        private void OnCloseStreaming()
        {
            StopCapture();
            _regionManager.Regions["StreamingRegion"].RemoveAll();
        }

        private void StopCapture()
        {
            _captureCts?.Cancel();
            _isStreaming = false;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            StopCapture();
        }

        public void Dispose()
        {
            StopCapture();
        }
    }
}
