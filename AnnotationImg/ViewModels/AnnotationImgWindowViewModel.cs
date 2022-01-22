using AnnotationImg.Commands.AnnotationImgWindow;
using AnnotationImg.Models;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Linq;

namespace AnnotationImg.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    class AnnotationImgWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ変更通知イベントハンドラ
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #region const
        /// <summary>
        /// Imageコントロールの最大の高さ
        /// </summary>
        private const double MAX_IMAGE_CONTROL_HEIGHT = 600;
        /// <summary>
        /// Imageコントロールの最大の幅
        /// </summary>
        private const double MAX_IMAGE_CONTROL_WIDTH = 1000;
        #endregion

        #region command
        public SelectFolderCommand SelectFolderCommand { get; private set; }
        public SelectImageCommand SelectNextImageCommand { get; private set; }
        public SelectImageCommand SelectPreviousImageCommand { get; private set; }
        public SelectImageCommand SelectHeadImageCommand { get; private set; }
        public SelectImageCommand SelectNextImageSaveCommand { get; private set; }
        public ExportFileCommand ExportFileCommand { get; private set; }
        #endregion

        /// <summary>
        /// BoundingBoxの境界線の色
        /// </summary>
        public string BorderBrushColor { get; private set; }

        private BitmapImage _image;

        /// <summary>
        /// 表示画像
        /// </summary>
        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged();
                }
            }
        }

        private ImageInfo _imageInfo;

        /// <summary>
        /// 表示画像情報
        /// </summary>
        public ImageInfo ImageInfo
        {
            get { return _imageInfo; }
            set
            {
                if (_imageInfo != value)
                {
                    _imageInfo = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FileName));
                    OnPropertyChanged(nameof(FileCount));
                    SelectImageCommandsRaiseCanExecuteChanged();
                }
            }
        }

        private int _imageIndex;

        /// <summary>
        /// 画像のインデックス
        /// </summary>
        public int ImageIndex
        {
            get { return _imageIndex; }
            set
            {
                if (_imageIndex != value)
                {
                    _imageIndex = value;
                    OnPropertyChanged();
                    SelectImageCommandsRaiseCanExecuteChanged();
                }
            }
        }

        private FolderInfo _folderInfo;

        /// <summary>
        /// フォルダ情報
        /// </summary>
        public FolderInfo FolderInfo
        {
            get { return _folderInfo; }
            set
            {
                if (_folderInfo != value)
                {
                    _folderInfo = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FileCount));
                    SelectImageCommandsRaiseCanExecuteChanged();
                    ExportFileCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _allIsChecked;

        /// <summary>
        /// すべてラジオボタン
        /// </summary>
        public bool AllIsChecked
        {
            get { return _allIsChecked; }
            set
            {
                if (_allIsChecked != value)
                {
                    _allIsChecked = value;
                    OnPropertyChanged();
                    SelectImageCommandsRaiseCanExecuteChanged();
                }
            }
        }

        private bool _untreatedIsChecked;

        /// <summary>
        /// 未処理ラジオボタン
        /// </summary>
        public bool UntreatedIsChecked
        {
            get { return _untreatedIsChecked; }
            set
            {
                if (_untreatedIsChecked != value)
                {
                    _untreatedIsChecked = value;
                    OnPropertyChanged();
                    SelectImageCommandsRaiseCanExecuteChanged();
                }
            }
        }

        private bool _treatedIsChecked;

        /// <summary>
        /// 処理済ラジオボタン
        /// </summary>
        public bool TreatedIsChecked
        {
            get { return _treatedIsChecked; }
            set
            {
                if (_treatedIsChecked != value)
                {
                    _treatedIsChecked = value;
                    OnPropertyChanged();
                    SelectImageCommandsRaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get => ImageInfo?.FileName; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileCount
        {
            get
            {
                if (FolderInfo is null)
                    return "";

                int alreadyCount = FolderInfo.Images
                    .Where(x => FolderInfo.TreatedImageIndexes.Contains(x.Index))
                    .Where(x => x.BoundingBoxes.Count > 0).Count();
                int excludeCount = FolderInfo.Images
                    .Where(x => FolderInfo.TreatedImageIndexes.Contains(x.Index))
                    .Where(x => x.BoundingBoxes.Count == 0).Count(); ;
                return $"全て {FolderInfo.Images.Length}, 済み{alreadyCount}、 除外{excludeCount}、 未{FolderInfo.UntreatedImageIndexes.Count}";
            }
        }

        /// <summary>
        /// Imageコントロールの最大の幅
        /// </summary>
        public double MaxImageControlWidth { get => MAX_IMAGE_CONTROL_WIDTH; }
        /// <summary>
        /// Imageコントロールの最大の高さ
        /// </summary>
        public double MaxImageControlHeight { get => MAX_IMAGE_CONTROL_HEIGHT; }

        /// <summary>
        /// 画像変更のコマンド実行の可能性を変更する
        /// </summary>
        private void SelectImageCommandsRaiseCanExecuteChanged()
        {
            this.SelectPreviousImageCommand.RaiseCanExecuteChanged();
            this.SelectHeadImageCommand.RaiseCanExecuteChanged();
            this.SelectNextImageCommand.RaiseCanExecuteChanged();
            this.SelectNextImageSaveCommand.RaiseCanExecuteChanged();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AnnotationImgWindowViewModel()
        {
            this.SelectFolderCommand = new SelectFolderCommand(this);
            this.SelectNextImageCommand = new SelectImageCommand(this, SelectImageCommand.SelectMode.Next);
            this.SelectPreviousImageCommand = new SelectImageCommand(this, SelectImageCommand.SelectMode.Previous);
            this.SelectHeadImageCommand = new SelectImageCommand(this, SelectImageCommand.SelectMode.Head);
            this.SelectNextImageSaveCommand = new SelectImageCommand(this, SelectImageCommand.SelectMode.NextAndSave);
            this.ExportFileCommand = new ExportFileCommand(this);

            this.BorderBrushColor = ConfigurationManager.AppSettings["BorderBrushColor"];

            this.AllIsChecked = true;
        }
    }
}
