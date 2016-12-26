using System.IO;
using UnityEditor;
using UnityEngine;

//version: 2016-12-26-00
namespace Jerry
{
    public class AssetImport : AssetPostprocessor
    {
        /// <summary>
        /// 查找一个AssetRule
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private AssetRule FindAssetRule(string path)
        {
            return SearchRecursive(path);
        }

        /// <summary>
        /// 当前目录查找，然后往上递归
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private AssetRule SearchRecursive(string path)
        {
            foreach (var findAsset in AssetDatabase.FindAssets("t:AssetRule", new[] { Path.GetDirectoryName(path) }))
            {
                var p = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(findAsset));
                if (p == Path.GetDirectoryName(path))
                {
                    string setName = string.Empty;
                    AssetRule rule = AssetDatabase.LoadAssetAtPath<AssetRule>(AssetDatabase.GUIDToAssetPath(findAsset));
                    if (rule != null && rule.IsMatch(assetImporter, out setName))
                    {
                        //Debug.LogWarning("Find:" + rule + " " + setName);
                        return rule;
                    }
                }
            }

            path = Directory.GetParent(path).FullName;
            path = path.Replace('\\', '/');
            path = path.Remove(0, Application.dataPath.Length);
            path = path.Insert(0, "Assets");
            if (path != "Assets")
            {
                return SearchRecursive(path);
            }
            return null;
        }

        private void OnPreprocessTexture()
        {
            AssetRule rule = FindAssetRule(assetImporter.assetPath);

            if (rule == null)
            {
                return;
            }

            rule.ApplySettings(assetImporter);
        }

        private void OnPreprocessModel()
        {
            AssetRule rule = FindAssetRule(assetImporter.assetPath);

            if (rule == null)
            {
                return;
            }

            rule.ApplySettings(assetImporter);
        }
    }
}