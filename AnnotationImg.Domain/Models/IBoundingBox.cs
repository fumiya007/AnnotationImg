namespace AnnotationImg.Domain
{
    /// <summary>
    /// Bounding Box
    /// </summary>
    public interface IBoundingBox
    {
        /// <summary>
        /// X座標
        /// </summary>
        int X { get; }
        /// <summary>
        /// Y座標
        /// </summary>
        int Y { get; }
        /// <summary>
        /// 幅
        /// </summary>
        int W { get; }
        /// <summary>
        /// 高さ
        /// </summary>
        int H { get; }
    }
}
