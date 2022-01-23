using System;
using AnnotationImg.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AnnotationImg.Views
{
    /// <summary>
    /// AnnotationImgWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AnnotationImgWindow : Window
    {
        private BoundingBox newBox;
        
        public AnnotationImgWindow()
        {
            InitializeComponent();
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddBoundingBox(e);
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RemoveIfSizeNotChanged();
        }

        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var boxies = canvas.Children.OfType<BoundingBox>();

            var point = e.GetPosition(canvas);
            var box = boxies.LastOrDefault(x => ToRectangle(x).Contains((int)point.X, (int)point.Y));

            if (box != null) canvas.Children.Remove(box);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            ResizeBoundingBox(sender, e);
        }

        /// <summary>
        /// キャンバス上で左クリックした位置にBoundingBoxを生成します。
        /// クリックした位置に既にBoundingBoxが存在する場合は処理を中止します。
        /// </summary>
        private void AddBoundingBox(MouseButtonEventArgs e)
        {
            var boxies = canvas.Children.OfType<BoundingBox>();

            var point = e.GetPosition(canvas);
            if (boxies.Any(x => ToRectangle(x).Contains((int)point.X, (int)point.Y)))
            {
                return;
            }

            newBox = new BoundingBox();
            newBox.Width = 0d;
            newBox.Height = 0d;
            canvas.Children.Add(newBox);
            Canvas.SetLeft(newBox, e.GetPosition(canvas).X);
            Canvas.SetTop(newBox, e.GetPosition(canvas).Y);
        }

        private static System.Drawing.Rectangle ToRectangle(BoundingBox box)
        {
            return new System.Drawing.Rectangle(
                (int)Canvas.GetLeft(box), (int)Canvas.GetTop(box), (int)box.ActualWidth, (int)box.ActualHeight);
        }

        /// <summary>
        ///左クリックで生成したBoundingBoxをドラッグ操作によりリサイズします。
        /// </summary>
        private void ResizeBoundingBox(object sender, MouseEventArgs e)
        {
            if (newBox == null)
                return;

            if (e.LeftButton == MouseButtonState.Released)
                return;

            newBox.Width = Math.Max(0, e.GetPosition(canvas).X - Canvas.GetLeft(newBox));
            newBox.Height = Math.Max(0, e.GetPosition(canvas).Y - Canvas.GetTop(newBox));
        }

        /// <summary>
        /// 生成したBoundingBoxがリサイズされなかった場合は削除します。
        /// </summary>
        private void RemoveIfSizeNotChanged()
        {
            if (newBox == null)
                return;

            if (newBox.ActualWidth == 0 || newBox.ActualHeight == 0)
                canvas.Children.Remove(newBox);

            newBox = null;
        }

        private void window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                return;

            var rate = e.Delta < 0 ? 0.9 : 1.1;
            var scaleTransform = canvas.RenderTransform as ScaleTransform;
            var scale = Math.Max(1d, scaleTransform.ScaleX * rate);

            scaleTransform.ScaleX = scale;
            scaleTransform.ScaleY = scale;
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var scaleTransform = canvas.RenderTransform as ScaleTransform;
            scaleTransform.ScaleX = 1d;
            scaleTransform.ScaleY = 1d;
        }
    }
}
