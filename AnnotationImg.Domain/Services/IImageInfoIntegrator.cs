using System.Collections.Generic;

namespace AnnotationImg.Domain
{
    public interface IImageInfoExporter
    {
        /// <summary>
        /// アノテーションの結果を出力する
        /// </summary>
        /// <param name="filePath">出力ファイルのパス</param>
        /// <param name="imageInfos">画像情報</param>
        void Export(string filePath, IEnumerable<IImageInfo> imageInfos);

        /// <summary>
        /// 出力ファイルのデフォルトのファイル名
        /// </summary>
        /// <returns>出力ファイルのデフォルトのファイル名</returns>
        string GetDefaultFileName();
    }
}
