using UnityEngine;
using UnityEditor;
using System.Collections;

public class ImportSetting_Texture : ImportSetting_Base
{
    private TextureImporter m_CurImpoter;

    public TextureImporterType m_textureType;
    public int m_maxTextureSize;
    public int m_textureFormat;

    public static ImportSetting_Texture CreateImportSetting_Texture(string name)
    {
        ImportSetting_Texture set = ImportSetting_Texture.CreateInstance<ImportSetting_Texture>();

        set.Init(name);

        return set;
    }

    public override void Draw()
    {
        base.Draw();

        EditorGUILayout.BeginVertical("box");

        this.m_textureType = (TextureImporterType)EditorGUILayout.EnumPopup("Texture Type", this.m_textureType);
        
        EditorGUILayout.EndVertical();
    }

    public override void Init(string name)
    {
        base.Init(name);
        m_TypeFilter = FilterType.TEXTURE;
    }

    public override bool ApplySettings(UnityEditor.AssetImporter importer)
    {
        if (base.ApplySettings(importer) == false)
        {
            return false;
        }

        m_CurImpoter = importer as TextureImporter;

        m_CurImpoter.textureType = m_textureType;

        return true;
    }
}