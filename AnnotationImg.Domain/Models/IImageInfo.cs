using System.Collections.Generic;

namespace AnnotationImg.Domain
{
    /// <summary>
    /// 画像情報
    /// </summary>
    public interface IImageInfo
    {
        /// <summary>
        /// ルートフォルダからみたファイル名
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// 幅
        /// </summary>
        int Width { get; }
        /// <summary>
        /// 高さ
        /// </summary>
        int Height { get; }
        /// <summary>
        /// バウンディングボックス
        /// </summary>
        IEnumerable<IBoundingBox> BoundingBoxes { get; }
    }
}
