using System;
using AnnotationImg.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnnotationImg.Views
{
    /// <summary>
    /// AnnotationImgWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AnnotationImgWindow : Window
    {
        private AnnotationImg.Controls.BoundingBox newBox;
        
        public AnnotationImgWindow()
        {
            InitializeComponent();
        }

        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var boxies = canvas.Children.OfType<BoundingBox>();

            var point = e.GetPosition(canvas);
            var box = boxies.LastOrDefault(x => ToRectangle(x).Contains((int)point.X, (int)point.Y));

            if (box is null)
            {
                AddBoundingBox(sender as Canvas, e);
            }
            else
            {
                canvas.Children.Remove(box);
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            ResizeBoundingBox(sender, e);
        }

        private void canvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (newBox == null)
                return;

            RemoveIfSizeNotChanged(sender as Canvas, newBox);

            newBox = null;
        }

        /// <summary>
        /// キャンバス上で右クリックした位置にBoundingBoxを生成します。
        /// </summary>
        private void AddBoundingBox(Canvas canvas, MouseButtonEventArgs e)
        {
            newBox = new BoundingBox();
            newBox.Width = 25;
            newBox.Height = 25;
            canvas.Children.Add(newBox);
            Canvas.SetLeft(newBox, e.GetPosition(canvas).X);
            Canvas.SetTop(newBox, e.GetPosition(canvas).Y);
        }

        private static System.Drawing.Rectangle ToRectangle(BoundingBox x)
        {
            return new System.Drawing.Rectangle(
                (int)Canvas.GetLeft(x), (int)Canvas.GetTop(x), (int)x.ActualWidth, (int)x.ActualHeight);
        }

        /// <summary>
        ///左クリックで生成したBoundingBoxをドラッグ操作によりリサイズします。
        /// </summary>
        private void ResizeBoundingBox(object sender, MouseEventArgs e)
        {
            if (newBox == null)
                return;

            if (e.RightButton == MouseButtonState.Released)
                return;

            var canvas = sender as Canvas;
            newBox.Width = Math.Max(25, e.GetPosition(canvas).X - Canvas.GetLeft(newBox));
            newBox.Height = Math.Max(25, e.GetPosition(canvas).Y - Canvas.GetTop(newBox));
        }

        /// <summary>
        /// リサイズされなかった場合は生成したBoundingBoxを削除
        /// </summary>
        private void RemoveIfSizeNotChanged(Canvas canvas, AnnotationImg.Controls.BoundingBox boundingBox)
        {
            if (boundingBox.ActualWidth == 0 || newBox.ActualHeight == 0)
                canvas.Children.Remove(boundingBox);
        }
    }
}
