using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlankApp1.Views
{
    /// <summary>
    /// Interaction logic for MainWindowChatHistory.xaml
    /// </summary>
    public partial class MainWindowChatHistory : UserControl
    {
        private Point _scrollStartPoint;
        private double _scrollStartOffset;
        public MainWindowChatHistory()
        {
            InitializeComponent();

            // Allow focus for keyboard scrolling
            this.Focusable = true;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Up) ChatScrollViewer.LineUp();
                else if (e.Key == Key.Down) ChatScrollViewer.LineDown();
                else if (e.Key == Key.PageUp) ChatScrollViewer.PageUp();
                else if (e.Key == Key.PageDown) ChatScrollViewer.PageDown();
                else if (e.Key == Key.Home) ChatScrollViewer.ScrollToTop();
                else if (e.Key == Key.End) ChatScrollViewer.ScrollToBottom();
            };

            // Drag to scroll functionality
            ChatScrollViewer.PreviewMouseLeftButtonDown += (s, e) =>
            {
                _scrollStartPoint = e.GetPosition(this);
                _scrollStartOffset = ChatScrollViewer.VerticalOffset;
                ChatScrollViewer.CaptureMouse();
            };

            ChatScrollViewer.PreviewMouseMove += (s, e) =>
            {
                if (ChatScrollViewer.IsMouseCaptured)
                {
                    Point currentPoint = e.GetPosition(this);
                    double delta = currentPoint.Y - _scrollStartPoint.Y;
                    ChatScrollViewer.ScrollToVerticalOffset(_scrollStartOffset - delta);
                }
            };

            ChatScrollViewer.PreviewMouseLeftButtonUp += (s, e) =>
            {
                ChatScrollViewer.ReleaseMouseCapture();
            };

            // Auto-scroll to bottom on new messages
            ChatScrollViewer.ScrollChanged += (s, e) =>
            {
                if (e.ExtentHeightChange > 0)
                {
                    ChatScrollViewer.ScrollToBottom();
                }
            };
        }
    }
}
