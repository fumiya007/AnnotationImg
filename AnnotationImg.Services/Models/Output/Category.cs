using System.Text.Json.Serialization;

namespace AnnotationImg.Services
{
    /// <summary>
    /// Category
    /// </summary>
    public class Category
    {
        /// <summary>
        /// カテゴリID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// supercategory
        /// </summary>
        [JsonPropertyName("supercategory")]
        public string Supercategory { get; set; }
    }
}
