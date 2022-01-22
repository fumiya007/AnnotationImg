using AnnotationImg.Domain;
using System.Collections.Generic;
using System.Linq;

namespace AnnotationImg.Services
{
    /// <summary>
    /// 出力データに変換するクラス
    /// </summary>
    class OutputConverter
    {
        /// <summary>
        /// 変換処理
        /// </summary>
        /// <param name="imageInfos">画像情報</param>
        /// <returns>出力データ</returns>
        public Output Convert(IEnumerable<IImageInfo> imageInfos)
        {
            var categories = new Category[] {
                    new Category(){
                        Id = 0,
                        Supercategory = "none",
                        Name = "face",
                    }
            };

            var images = new List<Image>();
            var annotations = new List<Annotation>();

            int imageIndex = 0;
            int annotationIndex = 0;

            foreach (var imageInfo in imageInfos)
            {
                if (!imageInfo.BoundingBoxes.Any())
                {
                    continue;
                }

                images.Add(new Image()
                {
                    Id = imageIndex,
                    FileName = imageInfo.FileName.Replace('\\', '/'),
                    Width = imageInfo.Width,
                    Height = imageInfo.Height
                });

                foreach (var bb in imageInfo.BoundingBoxes)
                {
                    annotations.Add(new Annotation()
                    {
                        Id = annotationIndex++,
                        BBox = new int[] { bb.X, bb.Y, bb.W, bb.H },
                        Segmentation = new int[][][] {
                            new int[][]{
                                new int[] { 0, 0 },
                                new int[] { 0, 0 },
                                new int[] { 0, 0 },
                                new int[] { 0, 0 }
                            }
                        },
                        ImageId = imageIndex,
                        Ignore = 0,
                        CategoryId = 0,
                        Iscrowd = 0,
                        Area = 0
                    });
                }

                imageIndex++;
            }

            return new Output()
            {
                Categories = categories,
                Images = images.ToArray(),
                Annotations = annotations.ToArray()
            };
        }
    }
}
