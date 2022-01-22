using System.Text.Json.Serialization;

namespace AnnotationImg.Services
{
    /// <summary>
    /// 出力データ
    /// </summary>
    class Output
    {
        /// <summary>
        /// images
        /// </summary>
        [JsonPropertyName("categories")]
        public Category[] Categories { get; set; }
        /// <summary>
        /// images
        /// </summary>
        [JsonPropertyName("images")]
        public Image[] Images { get; set; }
        /// <summary>
        /// annotations
        /// </summary>
        [JsonPropertyName("annotations")]
        public Annotation[] Annotations { get; set; }
    }
}
