using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Jerry
{
    public class JerryFile
    {
        /// <summary>
        /// 递归获得目录下的所有文件
        /// </summary>
        /// <param name="path">目录（支持多个目录合并）</param>
        public static List<string> GetFiles(List<string> paths)
        {
            List<string> filePath = new List<string>();

            foreach (string path in paths)
            {
                filePath.AddRange(Directory.GetFiles(path));

                foreach (string strDirectory in Directory.GetDirectories(path))
                {
                    filePath.AddRange(GetFiles(new List<string>() { strDirectory }));
                }
            }
            //Application.dataPath
            return filePath;
        }
    }
}