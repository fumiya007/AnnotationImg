using System.Text.Json.Serialization;

namespace AnnotationImg.Services
{
    /// <summary>
    /// Annotation
    /// </summary>
    public class Annotation
    {
        /// <summary>
        /// Annotationのid
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 矩形の座標
        /// </summary>
        [JsonPropertyName("bbox")]
        public int[] BBox { get; set; }
        /// <summary>
        /// segmentation
        /// </summary>
        [JsonPropertyName("segmentation")]
        public int[][][] Segmentation { get; set; }
        /// <summary>
        /// 画像のID
        /// </summary>
        [JsonPropertyName("image_id")]
        public int ImageId { get; set; }
        /// <summary>
        /// ignore
        /// </summary>
        [JsonPropertyName("ignore")]
        public int Ignore { get; set; }
        /// <summary>
        /// 分類のID
        /// </summary>
        [JsonPropertyName("category_id")]
        public int CategoryId { get; set; }
        /// <summary>
        /// iscrowd
        /// </summary>
        [JsonPropertyName("iscrowd")]
        public int Iscrowd { get; set; }
        /// <summary>
        /// area
        /// </summary>
        [JsonPropertyName("area")]
        public int Area { get; set; }
    }
}
