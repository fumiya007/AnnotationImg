using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AnnotationImg.Controls
{
    /// <summary>
    /// BoundingBox.xaml の相互作用ロジック
    /// </summary>
    public partial class BoundingBox : UserControl
    {
        private bool _isResizing;

        public BoundingBox()
        {
            InitializeComponent();
        }

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_isResizing) return;

            Image image = getImage();

            var left = Canvas.GetLeft(this) + e.HorizontalChange;
            left = Math.Max(left, 0);
            left = Math.Min(left, image.ActualWidth - ActualWidth);
            var top = Canvas.GetTop(this) + e.VerticalChange;
            top = Math.Max(top, 0);
            top = Math.Min(top, image.ActualHeight - ActualHeight);

            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);

            e.Handled = true;
        }

        void onDragStarted(object sender, DragStartedEventArgs e)
        {
        }

        void onDragCompleted(object sender, DragCompletedEventArgs e)
        {
        }

        private void onRisizing(object sender, DragDeltaEventArgs e)
        {
            Image image = getImage();

            double width = Math.Min(ActualWidth + e.HorizontalChange, image.ActualWidth - Canvas.GetLeft(this));
            Width = Math.Max(width, 10);
            double height = Math.Min(ActualHeight + e.VerticalChange, image.ActualHeight - Canvas.GetTop(this));
            Height = Math.Max(height, 10);
        }

        private void onRisizeStarted(object sender, DragStartedEventArgs e)
        {
            _isResizing = true;
        }

        private void onRisizeCompleted(object sender, DragCompletedEventArgs e)
        {
            _isResizing = false;
        }

        private Image getImage()
        {
            var canvas = this.Parent as Canvas;
            Image image = null;

            foreach (var item in canvas.Children)
            {
                if (item is Image)
                {
                    image = item as Image;
                    break;
                }
            }

            return image;
        }
    }
}