using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 资源过滤类型
/// </summary>
[System.Serializable]
public enum AssetFilterType
{
    /// <summary>
    /// 任何
    /// </summary>
    kAny,

    /// <summary>
    /// 贴图
    /// </summary>
    kTexture,

    /// <summary>
    /// Mesh
    /// </summary>
    kMesh,
}

/// <summary>
/// 资源规则
/// </summary>
[System.Serializable]
public class AssetRule : ScriptableObject
{
    public List<ImportSetting_Mesh> sets = new List<ImportSetting_Mesh>();

    /// <summary>
    /// 过滤器
    /// </summary>
    public AssetFilter filter;
    
    /// <summary>
    /// 设置
    /// </summary>
    public AssetRuleImportSettings settings;

    public static AssetRule CreateAssetRule()
    {
        var assetRule = AssetRule.CreateInstance<AssetRule>();

        assetRule.ApplyDefaults();

        return assetRule;
    }

    public void ApplyDefaults()
    {
        filter.ApplyDefaults();
        settings.ApplyDefaults();
    }

    public bool IsMatch(AssetImporter importer)
    {
        return filter.IsMatch(importer);
    }

    public bool AreSettingsCorrect(AssetImporter importer)
    {
        return settings.AreSettingsCorrect(importer);
    }

    /// <summary>
    /// 进行设置
    /// </summary>
    /// <param name="importer"></param>
    public void ApplySettings(AssetImporter importer)
    {
        settings.Apply(importer);
    }
}

/// <summary>
/// 资源过滤器
/// </summary>
[System.Serializable]
public struct AssetFilter
{
    /// <summary>
    /// 类型
    /// </summary>
    public AssetFilterType type;

    /// <summary>
    /// 路径
    /// </summary>
    public string path;

    /// <summary>
    /// 是否匹配
    /// </summary>
    /// <param name="importer"></param>
    /// <returns></returns>
    public bool IsMatch(AssetImporter importer)
    {
        if (importer == null)
        {
            return false;
        }

        AssetFilterType filterType = GetAssetFilterType(importer);
        return IsMatch(filterType, importer.assetPath);
    }

    public bool IsMatch(string path)
    {
        if (string.IsNullOrEmpty(this.path))
        {
            return true;
        }

        if (string.IsNullOrEmpty(path))
        {
            return string.IsNullOrEmpty(this.path);
        }

        string fullPath = Path.Combine(Application.dataPath, path);

        string[] files = Directory.GetFiles(Application.dataPath, this.path);
        if (files == null)
        {
            return false;
        }

        for (int i = 0; i < files.Length; i++)
        {
            if (fullPath.Equals(files[i]))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsMatch(AssetFilterType type, string path)
    {
        return (this.type == AssetFilterType.kAny || type == this.type) &&
            IsMatch(path);
    }

    /// <summary>
    /// 获取导入资源的类型
    /// </summary>
    /// <param name="importer"></param>
    /// <returns></returns>
    public static AssetFilterType GetAssetFilterType(AssetImporter importer)
    {
        if (importer is TextureImporter)
        {
            return AssetFilterType.kTexture;
        }
        else if (importer is ModelImporter)
        {
            return AssetFilterType.kMesh;
        }

        return AssetFilterType.kAny;
    }

    /// <summary>
    /// 默认设置
    /// </summary>
    public void ApplyDefaults()
    {
        type = AssetFilterType.kAny;
        path = string.Empty;
    }
}

/// <summary>
/// 资源导入设置规则
/// </summary>
[System.Serializable]
public struct AssetRuleImportSettings
{
    /// <summary>
    /// 贴图设置
    /// </summary>
    public TextureImporterSettings textureSettings;
    
    /// <summary>
    /// mesh设置
    /// </summary>
    public ImportSetting_Mesh meshSettings;

    public bool AreSettingsCorrect(AssetImporter importer)
    {
        //string[] aaa = "((_)|(+))&((0*1)|((1)&(2*0)))";

        if (importer is TextureImporter)
        {
            return AreSettingsCorrectTexture((TextureImporter)importer);
        }
        else if (importer is ModelImporter)
        {
            return AreSettingsCorrectModel((ModelImporter)importer);
        }
        return true;
    }

    bool AreSettingsCorrectTexture(TextureImporter importer)
    {
        TextureImporterSettings currentSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(currentSettings);

        return TextureImporterSettings.Equal(currentSettings, this.textureSettings);
    }

    bool AreSettingsCorrectModel(ModelImporter importer)
    {
        var currentSettings = ImportSetting_Mesh.Extract(importer);
        return ImportSetting_Mesh.Equal(currentSettings, this.meshSettings);
    }

    public void Apply(AssetImporter importer)
    {
        if (importer is TextureImporter)
        {
            ApplyTextureSettings((TextureImporter)importer);
        }
        else if (importer is ModelImporter)
        {
            ApplyMeshSettings((ModelImporter)importer);
        }
    }

    void ApplyTextureSettings(TextureImporter importer)
    {
        bool dirty = false;
        TextureImporterSettings tis = new TextureImporterSettings();
        importer.ReadTextureSettings(tis);
        if (!tis.mipmapEnabled == textureSettings.mipmapEnabled)
        {
            tis.mipmapEnabled = textureSettings.mipmapEnabled;
            dirty = true;
        }
        if (!tis.readable == textureSettings.readable)
        {
            tis.readable = textureSettings.readable;
            dirty = true;
        }
        if (tis.maxTextureSize != textureSettings.maxTextureSize)
        {
            tis.maxTextureSize = textureSettings.maxTextureSize;
            dirty = true;
        }

        // add settings as needed

        if (dirty)
        {
            Debug.Log("Modifying texture settings");
            importer.SetTextureSettings(tis);
            importer.SaveAndReimport();
        }
        else
        {
            Debug.Log("Texture Import Settings are Ok");
        }
    }

    void ApplyMeshSettings(ModelImporter importer)
    {
        bool dirty = false;
        if (importer.isReadable != meshSettings.readWriteEnabled)
        {
            importer.isReadable = meshSettings.readWriteEnabled;
            dirty = true;
        }

        if (importer.optimizeMesh != meshSettings.optimiseMesh)
        {
            importer.optimizeMesh = meshSettings.optimiseMesh;
            dirty = true;
        }

        if (importer.importBlendShapes != meshSettings.ImportBlendShapes)
        {
            importer.importBlendShapes = meshSettings.ImportBlendShapes;
            dirty = true;
        }

        // Add more settings in here that you might need

        if (dirty)
        {
            Debug.Log("Modifying Model Import Settings, An Import will now occur and the settings will be checked to be OK again during that import");
            importer.SaveAndReimport();
        }
        else
        {
            Debug.Log("Model Import Settings OK");
        }
    }

    /// <summary>
    /// 默认设置
    /// </summary>
    public void ApplyDefaults()
    {
        meshSettings = new ImportSetting_Mesh();
        meshSettings.ApplyDefaults();
        ApplyTextureSettingDefaults();
    }

    private void ApplyTextureSettingDefaults()
    {
        textureSettings = new TextureImporterSettings();
        textureSettings.maxTextureSize = 2048;
        textureSettings.mipmapEnabled = true;
    }
}
