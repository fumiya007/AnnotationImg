using System;
using System.Windows.Media.Imaging;

namespace AnnotationImg.Utils
{
    class ImageUtil
    {
        /// <summary>
        /// 画像取得
        /// </summary>
        /// <returns>ファイルパス</returns>
        public static BitmapImage GetBitmapImage(string fileName)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.None;
            image.UriSource = new Uri(fileName, UriKind.Absolute);
            image.EndInit();
            image.Freeze();
            return image;
        }

        /// <summary>
        /// 画像の大きさ取得
        /// </summary>
        /// <returns>大きさ</returns>
        public static (int width, int height) GetImageSize(string fileName)
        {
            int width, height;
            // 横幅と高さは画像を読み込んで設定
            using (var imageFile = System.Drawing.Image.FromFile(fileName))
            {
                width  = imageFile.Width;
                height = imageFile.Height;
            }

            return (width, height);
        }
    }
}
