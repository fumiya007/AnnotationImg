using AnnotationImg.Domain;

namespace AnnotationImg.Models
{
    public class BoundingBox : IBoundingBox
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
    }
}
