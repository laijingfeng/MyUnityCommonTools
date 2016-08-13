using System.IO;
using UnityEditor;
using UnityEngine;

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
        Debug.LogWarning("Path=" + path);
        foreach (var findAsset in AssetDatabase.FindAssets("t:AssetRule", new[] { Path.GetDirectoryName(path) }))
        {
            var p = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(findAsset));
            if (p == Path.GetDirectoryName(path))
            {
                Debug.Log("Found AssetRule : " + AssetDatabase.GUIDToAssetPath(findAsset));
                return AssetDatabase.LoadAssetAtPath<AssetRule>(AssetDatabase.GUIDToAssetPath(findAsset));
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