using System;
using System.Collections.Generic;
using System.Text;

namespace AnnotationImg.Utils
{
    /// <summary>
    /// 定数クラス
    /// </summary>
    static class Consts
    {
        /// <summary>
        /// 中間ファイル名
        /// </summary>
        public const string INTERMEDIATE_FILE_NAME = "{0}.json";
        /// <summary>
        /// BoundingBox最小サイズ
        /// </summary>
        public const double BOUNDING_BOX_MIN_SIZE = 30d;
    }
}
