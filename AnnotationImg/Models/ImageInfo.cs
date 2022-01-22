using System.Collections.Generic;
using System.Text.Json.Serialization;
using AnnotationImg.Domain;

namespace AnnotationImg.Models
{
    public class ImageInfo : IImageInfo
    {
        /// <summary>
        /// ファイルパス
        /// </summary>
        [JsonIgnore]
        public string FullFileName { get; set; }
        /// <summary>
        /// ルートフォルダからみたファイル名
        /// </summary>
        [JsonIgnore]
        public string FileName { get; set; }
        /// <summary>
        /// インデックス
        /// </summary>
        /// 
        private int _Index { get; set; }
        [JsonIgnore]
        public int Index
        {
            get => _Index;
            set => _Index = value;
        }
        /// <summary>
        /// ファイル名
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// ファイル名
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// バウンディングボックス
        /// </summary>
        public List<BoundingBox> BoundingBoxes { get; set; }

        IEnumerable<IBoundingBox> IImageInfo.BoundingBoxes => BoundingBoxes;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageInfo()
        {
            this.BoundingBoxes = new List<BoundingBox>();
        }
    }
}
