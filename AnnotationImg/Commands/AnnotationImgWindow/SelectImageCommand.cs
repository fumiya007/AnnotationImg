using AnnotationImg.ViewModels;
using AnnotationImg.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using AnnotationImg.Models;
using System.IO;
using System.Windows;
using System.Text.Json;
using System.Windows.Controls;

namespace AnnotationImg.Commands.AnnotationImgWindow
{
    class SelectImageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly AnnotationImgWindowViewModel viewModel;
        private readonly SelectMode mode;

        public enum SelectMode
        {
            Head,
            Previous,
            Next,
            NextAndSave,
        }

        public SelectImageCommand(AnnotationImgWindowViewModel viewModel, SelectMode mode)
        {
            this.viewModel = viewModel;
            this.mode = mode;
        }

        public bool CanExecute(object parameter)
        {
            switch (this.mode)
            {
                case SelectMode.Head:
                case SelectMode.Previous:
                case SelectMode.Next:
                    return existImage();
                case SelectMode.NextAndSave:
                    return true;
            }

            return false;
        }

        public void Execute(object parameter)
        {
            var canvas = parameter as Canvas;
            ImageInfo imageInfo = null;

            // 選択処理
            imageInfo = selectImage();

            System.Windows.Controls.Image imageControl = null;
            var cbbs = new List<Controls.BoundingBox>();

            foreach (var child in canvas.Children)
            {
                if (child is Image)
                {
                    imageControl = child as Image;
                }
                else if (child is Controls.BoundingBox)
                {
                    cbbs.Add(child as Controls.BoundingBox);
                }
            }

            // 保存処理
            if (this.mode == SelectMode.NextAndSave)
            {
                double wRate = this.viewModel.ImageInfo.Width / imageControl.ActualWidth;
                double hRate = this.viewModel.ImageInfo.Height / imageControl.ActualHeight;

                var mbbs = new List<Models.BoundingBox>();

                foreach (var cbb in cbbs)
                {
                    int x = (int)(Canvas.GetLeft(cbb) * wRate);
                    int y = (int)(Canvas.GetTop(cbb) * wRate);
                    int w = (int)(cbb.Width * wRate);
                    int h = (int)(cbb.Height * hRate);

                    // 画像の外にはみ出たBoundingBoxは範囲内に収める
                    if (x < 0)
                    {
                        w = Math.Max(w - x, 1);
                        x = 0;
                    }
                    else if (x >= this.viewModel.ImageInfo.Width)
                    {
                        x = this.viewModel.ImageInfo.Width - 1;
                        w = 1;
                    }

                    if (y < 0)
                    {
                        h = Math.Max(h - y, 1);
                        y = 0;
                    }
                    else if (y >= this.viewModel.ImageInfo.Height)
                    {
                        y = this.viewModel.ImageInfo.Height - 1;
                        h = 1;
                    }

                    if (x + w > this.viewModel.ImageInfo.Width)
                    {
                        w = w - ((x + w) - (int)this.viewModel.ImageInfo.Width);
                    }

                    if (h + y > this.viewModel.ImageInfo.Height)
                    {
                        h = h - ((h + y) - (int)this.viewModel.ImageInfo.Height);
                    }

                    mbbs.Add(new Models.BoundingBox()
                    {
                        X = x,
                        Y = y,
                        W = w,
                        H = h,
                    });
                }

                this.viewModel.ImageInfo.BoundingBoxes = mbbs;

                //書き込むファイルを開く
                using var sw = new System.IO.FileStream(
                    String.Format(Consts.INTERMEDIATE_FILE_NAME, this.viewModel.ImageInfo.FullFileName),
                    FileMode.Create);

                var option = new JsonWriterOptions()
                {
                    Indented = true
                };

                using var jw = new Utf8JsonWriter(sw, option);
                //シリアル化し、Jsonファイルに保存する
                JsonSerializer.Serialize<ImageInfo>(jw, viewModel.ImageInfo);

                if (!this.viewModel.FolderInfo.TreatedImageIndexes.Contains(viewModel.ImageInfo.Index))
                {
                    this.viewModel.FolderInfo.UntreatedImageIndexes.Remove(viewModel.ImageInfo.Index);
                    this.viewModel.FolderInfo.TreatedImageIndexes.Add(viewModel.ImageInfo.Index);
                    this.viewModel.FolderInfo.TreatedImageIndexes.Sort();
                }
            }

            // 画像切り替え処理
            if (!(imageInfo is null) && imageInfo != this.viewModel.ImageInfo)
            {
                this.viewModel.ImageInfo = imageInfo;
                cbbs.ForEach(x => canvas.Children.Remove(x));

                try
                {
                    this.viewModel.Image = ImageUtil.GetBitmapImage(this.viewModel.ImageInfo.FullFileName);

                    (this.viewModel.ImageInfo.Width, this.viewModel.ImageInfo.Height) = ImageUtil.GetImageSize(this.viewModel.ImageInfo.FullFileName);

                    // コントロール/モデルの比
                    double hCMRate = this.viewModel.MaxImageControlHeight / this.viewModel.ImageInfo.Height;
                    double wCMRate = this.viewModel.MaxImageControlWidth / this.viewModel.ImageInfo.Width;

                    double actualHeight, actualWidth;

                    if (this.viewModel.ImageInfo.Width * hCMRate > this.viewModel.MaxImageControlWidth)
                    {
                        actualWidth = this.viewModel.MaxImageControlWidth;
                        actualHeight = this.viewModel.ImageInfo.Height * wCMRate;
                    }
                    else
                    {
                        actualWidth = this.viewModel.ImageInfo.Width * hCMRate;
                        actualHeight = this.viewModel.MaxImageControlHeight;
                    }

                    double wRate = actualWidth / this.viewModel.ImageInfo.Width;
                    double hRate = actualHeight / this.viewModel.ImageInfo.Height;

                    foreach (var item in imageInfo.BoundingBoxes)
                    {
                        var b = new Controls.BoundingBox();
                        b.Width = item.W * wRate;
                        b.Height = item.H * hRate;
                        Canvas.SetLeft(b, item.X * wRate);
                        Canvas.SetTop(b, item.Y * hRate);
                        canvas.Children.Add(b);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.viewModel.Image = null;
                }
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 画像選択処理
        /// </summary>
        /// <returns>表示する画像情報</returns>
        private ImageInfo selectImage()
        {
            if (!existImage())
            {
                return null;
            }

            switch (this.mode)
            {
                case SelectMode.Head:
                    if (this.viewModel.AllIsChecked)
                    {
                        this.viewModel.ImageIndex = 0;
                        return this.viewModel.FolderInfo.Images.First(x => x.Index == 0);
                    }
                    else if (this.viewModel.UntreatedIsChecked)
                    {
                        int indexH = this.viewModel.FolderInfo.UntreatedImageIndexes[0];
                        this.viewModel.ImageIndex = indexH;
                        return this.viewModel.FolderInfo.Images.First(x => x.Index == indexH);
                    }
                    else if (this.viewModel.TreatedIsChecked)
                    {
                        int indexH = this.viewModel.FolderInfo.TreatedImageIndexes[0];
                        this.viewModel.ImageIndex = indexH;
                        return this.viewModel.FolderInfo.Images.First(x => x.Index == indexH);
                    }
                    break;
                case SelectMode.Previous:
                    int indexP = 0;
                    if (this.viewModel.AllIsChecked)
                    {
                        indexP = this.viewModel.ImageIndex - 1;
                    }
                    else if (this.viewModel.UntreatedIsChecked)
                    {
                        for (int i = 0; i < this.viewModel.FolderInfo.UntreatedImageIndexes.Count(); i++)
                        {
                            if (this.viewModel.ImageIndex <= this.viewModel.FolderInfo.UntreatedImageIndexes[i])
                            {
                                break;
                            }
                            indexP = this.viewModel.FolderInfo.UntreatedImageIndexes[i];
                        }

                        this.viewModel.ImageIndex = indexP;
                        return this.viewModel.FolderInfo.Images.First(x => x.Index == indexP);
                    }
                    else if (this.viewModel.TreatedIsChecked)
                    {
                        for (int i = 0; i < this.viewModel.FolderInfo.TreatedImageIndexes.Count(); i++)
                        {
                            if (this.viewModel.ImageIndex <= this.viewModel.FolderInfo.TreatedImageIndexes[i])
                            {
                                break;
                            }
                            indexP = this.viewModel.FolderInfo.TreatedImageIndexes[i];
                        }

                    }
                    this.viewModel.ImageIndex = indexP;
                    return this.viewModel.FolderInfo.Images.First(x => x.Index == indexP);
                case SelectMode.Next:
                case SelectMode.NextAndSave:
                    int indexN = 0;
                    if (this.viewModel.AllIsChecked)
                    {
                        indexN = this.viewModel.ImageIndex + 1;
                    }
                    else if (this.viewModel.UntreatedIsChecked)
                    {
                        for (int i = 0; i < this.viewModel.FolderInfo.UntreatedImageIndexes.Count(); i++)
                        {
                            if (this.viewModel.ImageIndex < this.viewModel.FolderInfo.UntreatedImageIndexes[i])
                            {
                                indexN = this.viewModel.FolderInfo.UntreatedImageIndexes[i];
                                break;
                            }
                        }

                        this.viewModel.ImageIndex = indexN;
                        return this.viewModel.FolderInfo.Images.First(x => x.Index == indexN);
                    }
                    else if (this.viewModel.TreatedIsChecked)
                    {
                        for (int i = 0; i < this.viewModel.FolderInfo.TreatedImageIndexes.Count(); i++)
                        {
                            if (this.viewModel.ImageIndex < this.viewModel.FolderInfo.TreatedImageIndexes[i])
                            {
                                indexN = this.viewModel.FolderInfo.TreatedImageIndexes[i];
                                break;
                            }
                        }

                    }
                    this.viewModel.ImageIndex = indexN;
                    return this.viewModel.FolderInfo.Images.First(x => x.Index == indexN);
            }
            return null;
        }

        /// <summary>
        /// 表示する画像が存在するかどうか
        /// </summary>
        /// <returns>存在する場合はtrue</returns>
        private bool existImage()
        {
            if (this.viewModel.FolderInfo is null ||
                this.viewModel.FolderInfo.Images is null) return false;

            switch (this.mode)
            {
                case SelectMode.Head:
                    if (this.viewModel.AllIsChecked)
                    {
                        return true;
                    }
                    else if (this.viewModel.UntreatedIsChecked)
                    {
                        return this.viewModel.FolderInfo.UntreatedImageIndexes.Any();
                    }
                    else if (this.viewModel.TreatedIsChecked)
                    {
                        return this.viewModel.FolderInfo.TreatedImageIndexes.Any();
                    }
                    break;
                case SelectMode.Previous:
                    if (this.viewModel.AllIsChecked)
                    {
                        return this.viewModel.ImageIndex > 0;
                    }
                    else if (this.viewModel.UntreatedIsChecked)
                    {
                        return this.viewModel.FolderInfo.UntreatedImageIndexes
                            .Any(x => x < this.viewModel.ImageIndex);
                    }
                    else if (this.viewModel.TreatedIsChecked)
                    {
                        return this.viewModel.FolderInfo.TreatedImageIndexes
                            .Any(x => x < this.viewModel.ImageIndex);
                    }
                    break;
                case SelectMode.Next:
                case SelectMode.NextAndSave:

                    if (this.viewModel.AllIsChecked)
                    {
                        return this.viewModel.ImageIndex < this.viewModel.FolderInfo.Images.Length - 1;
                    }
                    else if (this.viewModel.UntreatedIsChecked)
                    {
                        return this.viewModel.FolderInfo.UntreatedImageIndexes
                            .Any(x => x > this.viewModel.ImageIndex);
                    }
                    else if (this.viewModel.TreatedIsChecked)
                    {
                        return this.viewModel.FolderInfo.TreatedImageIndexes
                            .Any(x => x > this.viewModel.ImageIndex);
                    }
                    break;
            }
            return false;
        }
    }
}
