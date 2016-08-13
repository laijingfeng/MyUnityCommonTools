using System;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ImportSetting_Mesh : ScriptableObject
{
    public bool readWriteEnabled;
    public bool optimiseMesh;
    public bool ImportBlendShapes;
    public static Type m_ImpoterType = typeof(ModelImporter);

    /// <summary>
    /// 提取设置
    /// </summary>
    /// <param name="importer"></param>
    /// <returns></returns>
    public static ImportSetting_Mesh Extract(ModelImporter importer)
    {
        if (importer == null)
        {
            throw new ArgumentException();
        }

        ImportSetting_Mesh settings = new ImportSetting_Mesh();
        settings.readWriteEnabled = importer.isReadable;
        settings.optimiseMesh = importer.optimizeMesh;
        settings.ImportBlendShapes = importer.importBlendShapes;
        return settings;
    }

    /// <summary>
    /// 是否相等
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Equal(ImportSetting_Mesh a, ImportSetting_Mesh b)
    {
        return (a.readWriteEnabled == b.readWriteEnabled) && (a.optimiseMesh == b.optimiseMesh) && (a.ImportBlendShapes == b.ImportBlendShapes);
    }

    public bool ApplySettins(AssetImporter importer)
    {
        UnityEngine.Debug.LogWarning("111 " + importer.GetType());
        if (importer.GetType().Equals(m_ImpoterType) == false)
        {
            
            return false;
        }
        return true;
    }

    /// <summary>
    /// 默认设置
    /// </summary>
    public void ApplyDefaults()
    {
        readWriteEnabled = false;
        optimiseMesh = true;
        ImportBlendShapes = false;
    }

    public void Draw()
    {
        GUILayout.Space(20);
        GUILayout.Label("MESH SETTINGS");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Read/Write Enabled");
        readWriteEnabled = EditorGUILayout.Toggle(readWriteEnabled);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Optimise Mesh");
        optimiseMesh = EditorGUILayout.Toggle(optimiseMesh);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Blend Shapes");
        ImportBlendShapes = EditorGUILayout.Toggle(ImportBlendShapes);
        EditorGUILayout.EndHorizontal();
    }
}