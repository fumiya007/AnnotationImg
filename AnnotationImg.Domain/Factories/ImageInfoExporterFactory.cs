using AnnotationImg.Domain;

namespace AnnotationImg
{
    public static class ImageInfoExporterFactory
    {
        private static IImageInfoExporter _integrator;
        public static void Initialize(IImageInfoExporter integrator)
        {
            _integrator = integrator;
        }
        public static IImageInfoExporter GetService()
        {
            return _integrator;
        }
    }
}
