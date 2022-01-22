using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using AnnotationImg.Domain;

namespace AnnotationImg.Services
{
    public class AnnotationImgExporter : IImageInfoExporter
    {
        /// <summary>
        /// デフォルトファイル名取得
        /// </summary>
        /// <returns>デフォルトファイル名</returns>
        public string GetDefaultFileName()
        {
            return "data.json";
        }

        /// <summary>
        /// 出力結果を出力する
        /// </summary>
        /// <param name="filePath">出力ファイルパス</param>
        /// <param name="imageInfos">画像情報</param>
        public void Export(string filePath, IEnumerable<IImageInfo> imageInfos)
        {
            var converter = new OutputConverter();
            var output = converter.Convert(imageInfos);

            using var sw = new System.IO.FileStream(filePath, FileMode.Create);

            var option = new JsonWriterOptions()
            {
                Indented = false
            };

            using var jw = new Utf8JsonWriter(sw, option);
            JsonSerializer.Serialize<Output>(jw, output);
        }
    }
}
