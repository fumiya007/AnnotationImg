using System.Text.Json.Serialization;

namespace AnnotationImg.Services
{
    /// <summary>
    /// Image
    /// </summary>
    public class Image
    {
        /// <summary>
        /// 画像のID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// (ベースフォルダからの)ファイルパス
        /// </summary>
        [JsonPropertyName("file_name")]
        public string FileName { get; set; }
        /// <summary>
        /// 画像の幅
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }
        /// <summary>
        /// 画像の高さ
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }
    }
}
