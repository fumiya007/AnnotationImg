using AnnotationImg.ViewModels;
using AnnotationImg.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using AnnotationImg.Utils;
using System.Windows.Controls;

namespace AnnotationImg.Commands.AnnotationImgWindow
{
    class SelectFolderCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private AnnotationImgWindowViewModel viewModel;

        public SelectFolderCommand(AnnotationImgWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            using var cofd = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください",
                IsFolderPicker = true,
            };
            if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            var canvas = parameter as Canvas;

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

            this.viewModel.ImageIndex = 0;
            this.viewModel.FolderInfo = FolderInfo.Load(cofd.FileName);

            if (this.viewModel.FolderInfo.Images.Any())
            {
                this.viewModel.ImageInfo = this.viewModel.FolderInfo.Images[0];
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

                    foreach (var item in this.viewModel.ImageInfo.BoundingBoxes)
                    {
                        var b = new Controls.BoundingBox();
                        b.Width = item.W * wRate;
                        b.Height = item.H * hRate;
                        Canvas.SetLeft(b, item.X * wRate);
                        Canvas.SetTop(b, item.Y * hRate);
                        canvas.Children.Add(b);
                    }

                    viewModel.AllIsChecked = true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.viewModel.Image = null;
                }
            }
            else
            {
                this.viewModel.ImageInfo = null;
                this.viewModel.Image = null;
            }
        }
    }
}
