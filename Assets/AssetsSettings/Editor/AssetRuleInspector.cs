using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AssetRule))]
public class AssetRuleInspector : Editor
{
    [MenuItem("Assets/Create_AssetRule")]
    public static void CreateAssetRule()
    {
        AssetRule newRule = AssetRule.CreateAssetRule();

        string selectionPath = "Assets";
        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            selectionPath = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(selectionPath))
            {
                selectionPath = Path.GetDirectoryName(selectionPath);
            }
            break;
        }

        string newRuleFileName = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(selectionPath, "NewAssetRule.asset"));
        newRuleFileName = newRuleFileName.Replace("\\", "/");
        AssetDatabase.CreateAsset(newRule, newRuleFileName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newRule;
    }

    private AssetRule m_CurRule;
    private int m_SelectedID = 0;

    public override void OnInspectorGUI()
    {
        m_CurRule = (AssetRule)target;

        DrawAddBtn();

        GUILayout.Space(20);
        m_CurRule.showLog = EditorGUILayout.Toggle(new GUIContent("ShowLog"), m_CurRule.showLog);
        string[] names = GetNames();
        if (names != null && names.Length > 0)
        {
            m_SelectedID = EditorGUILayout.Popup("Sets", m_SelectedID, names);
        }

        DrawDelete();

        DrawSelect();

        serializedObject.ApplyModifiedProperties();
    }

    private ImportSetting_Base.FilterType m_AddFilterType;

    /// <summary>
    /// 绘制增加按钮
    /// </summary>
    private void DrawAddBtn()
    {
        EditorGUILayout.BeginHorizontal();

        m_AddFilterType = (ImportSetting_Base.FilterType)EditorGUILayout.EnumPopup(m_AddFilterType);

        GUI.color = Color.green;

        if (GUILayout.Button("+"))
        {
            AddItem();
        }

        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();
    }

    private string[] GetNames()
    {
        List<string> ss = new List<string>();
        if (m_CurRule.sets == null || m_CurRule.sets.Count < 1)
        {
            return ss.ToArray();
        }
        foreach (ImportSetting_Base im in m_CurRule.sets)
        {
            ss.Add(im.m_MyName);
        }
        return ss.ToArray();
    }

    private string NewName(ImportSetting_Base.FilterType type)
    {
        string pre = string.Empty;
        switch (type)
        {
            case ImportSetting_Base.FilterType.MODEL:
                {
                    pre = "model";
                }
                break;
            case ImportSetting_Base.FilterType.TEXTURE:
                {
                    pre = "texture";
                }
                break;
        }
        string ret = string.Format("{0}_{1}", pre, 0);
        if (m_CurRule.sets != null && m_CurRule.sets.Count > 0)
        {
            for (int i = 0; i < 1000; i++)
            {
                ret = string.Format("{0}_{1}", pre, i);
                if (m_CurRule.sets.Exists((x) => x.m_MyName.Equals(ret)) == false)
                {
                    break;
                }
            }
        }
        return ret;
    }

    private string CheckName(string name, int idx)
    {
        if (idx < 0 || idx >= m_CurRule.sets.Count)
        {
            return name;
        }
        
        if(string.IsNullOrEmpty(name))
        {
            return NewName(m_CurRule.sets[idx].m_TypeFilter);
        }

        bool finish = false;
        while(finish == false)
        {
            finish = true;
            for (int i = 0; i < m_CurRule.sets.Count; i++)
            {
                if (i == idx)
                {
                    continue;
                }

                if (m_CurRule.sets[i].m_MyName.Equals(name))
                {
                    finish = false;
                    name = name + "_1";
                    break;
                }
            }
        }
        
        return name;
    }

    private void AddItem()
    {
        switch (m_AddFilterType)
        {
            case ImportSetting_Base.FilterType.MODEL:
                {
                    m_CurRule.sets.Add(ImportSetting_Model.CreateImportSetting_Model(NewName(m_AddFilterType)));
                }
                break;
            case ImportSetting_Base.FilterType.TEXTURE:
                {
                    m_CurRule.sets.Add(ImportSetting_Texture.CreateImportSetting_Texture(NewName(m_AddFilterType)));
                }
                break;
        }
        m_SelectedID = m_CurRule.sets.Count - 1;
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDelete()
    {
        if (m_CurRule == null
            || m_CurRule.sets == null
            || m_SelectedID < 0
            || m_SelectedID >= m_CurRule.sets.Count)
        {
            return;
        }

        GUILayout.Space(10);

        GUI.color = Color.red;

        if (GUILayout.Button("-"))
        {
            m_CurRule.sets.RemoveAt(m_SelectedID);
            serializedObject.ApplyModifiedProperties();
            if (m_SelectedID > 0)
            {
                m_SelectedID--;
            }
        }

        GUI.color = Color.white;

        GUILayout.Space(10);
    }

    private void DrawSelect()
    {
        if (m_CurRule == null
            || m_CurRule.sets == null
            || m_SelectedID < 0
            || m_SelectedID >= m_CurRule.sets.Count)
        {
            return;
        }

        EditorGUILayout.BeginVertical("box");
        m_CurRule.sets[m_SelectedID].Draw();
        EditorGUILayout.EndVertical();

        m_CurRule.sets[m_SelectedID].m_MyName = CheckName(m_CurRule.sets[m_SelectedID].m_MyName, m_SelectedID);
    }
}