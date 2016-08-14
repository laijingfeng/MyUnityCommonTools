using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class ImportSetting_Base : ScriptableObject
{
    public enum FilterType
    {
        MODEL,
        TEXTURE,
    }

    public bool m_InUse;
    public FilterType m_TypeFilter;
    public string m_PathFilter;
    public string m_Name;

    public virtual void Draw()
    {
        EditorGUILayout.BeginVertical("box");

        this.m_InUse = EditorGUILayout.Toggle("InUse", this.m_InUse);

        EditorGUILayout.LabelField("FilterType", this.m_TypeFilter.ToString());

        this.m_Name = EditorGUILayout.TextField("Name", this.m_Name);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("PathFilter");
        this.m_PathFilter = EditorGUILayout.TextArea(this.m_PathFilter);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    public virtual bool Match(AssetImporter importer)
    {
        if (m_InUse == false)
        {
            return false;
        }
        if (importer == null)
        {
            return false;
        }
        switch (m_TypeFilter)
        {
            case FilterType.MODEL:
                {
                    if (!(importer is ModelImporter))
                    {
                        return false;
                    }
                }
                break;
            case FilterType.TEXTURE:
                {
                    if (!(importer is TextureImporter))
                    {
                        return false;
                    }
                }
                break;
        }
        return CheckPath(importer.assetPath, m_PathFilter);
    }

    private bool CheckPath(string path, string filter)
    {
        if (string.IsNullOrEmpty(filter) || string.IsNullOrEmpty(path))
        {
            return true;
        }
        return true;
    }

    public virtual void Init(string name)
    {
        m_InUse = true;
        m_PathFilter = string.Empty;
        m_Name = name;
    }

    /// <param name="importer"></param>
    /// <returns></returns>
    public virtual bool ApplySettings(AssetImporter importer)
    {
        return true;
    }
}
