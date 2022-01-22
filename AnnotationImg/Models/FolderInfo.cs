using System.IO;
using System.Linq;
using AnnotationImg.Utils;
using System;
using System.Text.Json;
using System.Collections.Generic;

namespace AnnotationImg.Models
{
    /// <summary>
    /// フォルダ情報
    /// </summary>
    public class FolderInfo
    {
        /// <summary>
        /// 画像ファイルとして取り込むファイルの拡張子
        /// </summary>
        private static readonly string[] IMG_EXTENSION = new string[] { ".png", ".jpg" };
        /// <summary>
        /// 全ての画像
        /// </summary>
        public ImageInfo[] Images { get; set; }
        /// <summary>
        /// 処理済みの画像
        /// </summary>
        public List<int> TreatedImageIndexes { get; set; }
        /// <summary>
        /// 未処理の画像
        /// </summary>
        public List<int> UntreatedImageIndexes { get; set; }
        /// <summary>
        /// フォルダ名
        /// </summary>
        public string FolderName { get; set; }
        /// <summary>
        /// フォルダ情報読み込み
        /// </summary>
        /// <param name="src">フォルダ名</param>
        public static FolderInfo Load(string src)
        {
            var folderInfo = new FolderInfo();
            folderInfo.FolderName = src;
            folderInfo.TreatedImageIndexes = new List<int>();
            folderInfo.UntreatedImageIndexes = new List<int>();
            var dir = new DirectoryInfo(src);

            // サブフォルダから取得する
            folderInfo.Images = dir.GetFiles("*", SearchOption.AllDirectories)
                .Where(x => IMG_EXTENSION.Any(y => y == x.Extension))
                .Select((x, i) => new ImageInfo()
                {
                    FullFileName = x.FullName,
                    FileName = x.FullName.Substring(src.Length + 1),
                    Index = i
                }
                ).ToArray();

            for (int i = 0; i < folderInfo.Images.Length; i++)
            {
                string dataFileName = String.Format(Consts.INTERMEDIATE_FILE_NAME, folderInfo.Images[i].FullFileName);
                if (File.Exists(dataFileName))
                {
                    ImageInfo imageInfoFromFile;

                    using (var reader = new StreamReader(dataFileName))
                    {
                        imageInfoFromFile = JsonSerializer.Deserialize<ImageInfo>(reader.ReadToEnd());
                    }

                    imageInfoFromFile.FullFileName = folderInfo.Images[i].FullFileName;
                    imageInfoFromFile.FileName = folderInfo.Images[i].FileName;
                    imageInfoFromFile.Index = i;
                    folderInfo.Images[i] = imageInfoFromFile;
                    folderInfo.TreatedImageIndexes.Add(i);
                }
                else
                {
                    folderInfo.UntreatedImageIndexes.Add(i);
                }
            }

            folderInfo.TreatedImageIndexes.Sort();
            folderInfo.UntreatedImageIndexes.Sort();

            folderInfo.FolderName = src;
            return folderInfo;
        }
    }
}
