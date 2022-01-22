using AnnotationImg.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows.Input;
using System.Windows;

namespace AnnotationImg.Commands.AnnotationImgWindow
{
    class ExportFileCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private AnnotationImgWindowViewModel viewModel;

        public ExportFileCommand(AnnotationImgWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return viewModel.FolderInfo != null &&
                   ImageInfoExporterFactory.GetService() != null;
        }

        public void Execute(object parameter)
        {
            var service = ImageInfoExporterFactory.GetService();

            if (service is null)
            {
                MessageBox.Show("サービスが存在しません。");
                return;
            }

            using var cofd = new CommonOpenFileDialog()
            {
                Title = "出力先を選択してください",
                DefaultFileName = service.GetDefaultFileName()
            };

            if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            try
            {
                service.Export(cofd.FileName, viewModel.FolderInfo.Images);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
