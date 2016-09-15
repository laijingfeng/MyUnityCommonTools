using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class AssetRule : ScriptableObject
{
    public bool showLog;
    public List<ImportSetting_Base> sets;

    public static AssetRule CreateAssetRule()
    {
        AssetRule assetRule = AssetRule.CreateInstance<AssetRule>();

        assetRule.Init();

        return assetRule;
    }

    public void Init()
    {
        sets = new List<ImportSetting_Base>();
    }

    public bool IsMatch(AssetImporter importer,out string setName)
    {
        setName = string.Empty;
        if (sets == null || sets.Count < 1)
        {
            return false;
        }
        foreach (ImportSetting_Base s in sets)
        {
            if (s.Match(importer))
            {
                setName = s.m_MyName;
                if (showLog)
                {
                    Debug.Log(string.Format("<color=white>{0}</color> <color=yellow>{1}.{2}</color>", importer.assetPath, this.name, setName));
                }
                return true;
            }
        }
        return false;
    }

    public bool ApplySettings(AssetImporter importer)
    {
        if (sets == null || sets.Count < 1)
        {
            return false;
        }
        foreach (ImportSetting_Base s in sets)
        {
            if (s.Match(importer))
            {
                s.ApplySettings(importer);
                return true;
            }
        }
        return false;
    }
}