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
        var newRule = CreateInstance<AssetRule>();
        newRule.ApplyDefaults();

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
        changed = true;
    }

    /// <summary>
    /// 删除的索引
    /// </summary>
    private int m_RemoveIndex = -1;

    /// <summary>
    /// 添加按钮文本
    /// </summary>
    private GUIContent m_IconToolbarPlus;

    /// <summary>
    /// 移除按钮文本
    /// </summary>
    private GUIContent m_IconToolbarMinus;

    private AssetRule m_CurRule;

    public override void OnInspectorGUI()
    {
        var t = (AssetRule)target;

        m_CurRule = t;

        EditorGUI.BeginChangeCheck();

        

        EditorGUILayout.BeginHorizontal();
        
        
        EditorGUILayout.LabelField("Asset Rule Type");

        t.filter.type = (AssetFilterType)EditorGUILayout.EnumPopup(t.filter.type);

        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            changed = true;
        }

        DrawAddBtn();

        DrawList();

        switch (t.filter.type)
        {
            case AssetFilterType.kAny:
                {
                    DrawTextureSettings(t);
                    DrawMeshSettings(t);
                }
                break;

            case AssetFilterType.kMesh:
                {
                    DrawMeshSettings(t);
                }
                break;

            case AssetFilterType.kTexture:
                {
                    DrawTextureSettings(t);
                }
                break;
        }

        if (changed)
        {
            if (GUILayout.Button("Apply"))
            {
                Apply(t);
            }
        }
    }

    /// <summary>
    /// 绘制增加按钮
    /// </summary>
    private void DrawAddBtn()
    {
        //计算添加按钮(x,y,width,height)
        Rect btPosition = GUILayoutUtility.GetRect(m_IconToolbarPlus, GUI.skin.button);//得到宽度是整个区域的宽度
        const float addButonWidth = 150f;
        btPosition.x = btPosition.x + (btPosition.width - addButonWidth) / 2;
        btPosition.width = addButonWidth;

        if (GUI.Button(btPosition, m_IconToolbarPlus))
        {
            AddItem();
        }

        //if (npcList.arraySize <= 0)
        //{
        //    EditorGUILayout.HelpBox("this list has no item", MessageType.Warning);
        //}
    }

    /// <summary>
    /// 向npcList添加一项
    /// </summary>
    private void AddItem()
    {
        //增加
        m_CurRule.sets.Add(new ImportSetting_Mesh());

        //serializedObject.ApplyModifiedProperties();
    }

    private void DrawList()
    {
        if(m_RemoveIndex > -1)
        {

        }

        for (int i = 0; i < m_CurRule.sets.Count; i++)
        {
            //Debug.LogWarning("1");
            m_CurRule.sets[i].Draw();
        }
    }

    private void Apply(AssetRule assetRule)
    {
        // get the directories that we do not want to apply changes to 
        List<string> dontapply = new List<string>();
        var assetrulepath = AssetDatabase.GetAssetPath(assetRule).Replace(assetRule.name + ".asset", "").TrimEnd('/');
        string projPath = Application.dataPath;
        projPath = projPath.Remove(projPath.Length - 6);

        string[] directories = Directory.GetDirectories(Path.GetDirectoryName(projPath + AssetDatabase.GetAssetPath(assetRule)), "*", SearchOption.AllDirectories);
        foreach (var directory in directories)
        {
            var d = directory.Replace(Application.dataPath, "Assets");
            var appDirs = AssetDatabase.FindAssets("t:AssetRule", new[] { d });
            if (appDirs.Length != 0)
            {
                d = d.TrimEnd('/');
                d = d.Replace('\\', '/');
                dontapply.Add(d);
            }
        }

        List<string> finalAssetList = new List<string>();
        foreach (var findAsset in AssetDatabase.FindAssets("", new[] { assetrulepath }))
        {
            var asset = AssetDatabase.GUIDToAssetPath(findAsset);
            if (!File.Exists(asset)) continue;
            if (dontapply.Contains(Path.GetDirectoryName(asset))) continue;
            if (!assetRule.IsMatch(AssetImporter.GetAtPath(asset))) continue;
            if (finalAssetList.Contains(asset)) continue;
            if (asset == AssetDatabase.GetAssetPath(assetRule)) continue;
            finalAssetList.Add(asset);
        }

        int i = 1;
        foreach (var asset in finalAssetList)
        {
            AssetImporter.GetAtPath(asset).SaveAndReimport();
            i++;
        }

        changed = false;
    }

    private void DrawMeshSettings(AssetRule assetRule)
    {
        m_CurRule.settings.meshSettings.Draw();
        //assetRule.settings.meshSettings.Draw();
    }

    private int[] sizes = new[] { 32, 64, 128, 256, 512, 1024, 2048, 4096 };
    private string[] sizeStrings = new[] { "32", "64", "128", "256", "512", "1024", "2048", "4096" };
    private static bool changed = false;

    private void DrawTextureSettings(AssetRule assetRule)
    {
        GUILayout.Space(20);
        GUILayout.Label(" TEXTURE SETTINGS ");
        GUILayout.Space(20);

        GetValidPlatforms();

        // mip maps
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Generate Mip Maps");
        assetRule.settings.textureSettings.mipmapEnabled = EditorGUILayout.Toggle(assetRule.settings.textureSettings.mipmapEnabled);
        EditorGUILayout.EndHorizontal();

        //read write enabled
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Read/Write Enabled");
        assetRule.settings.textureSettings.readable = EditorGUILayout.Toggle(assetRule.settings.textureSettings.readable);
        EditorGUILayout.EndHorizontal();

        // per platform settings
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Max Texture Size");
        assetRule.settings.textureSettings.maxTextureSize =
        EditorGUILayout.IntPopup(assetRule.settings.textureSettings.maxTextureSize, sizeStrings, sizes);
        EditorGUILayout.EndHorizontal();
    }

    private void GetValidPlatforms()
    {

    }

    void OnEnable()
    {
        m_IconToolbarPlus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus"));
        m_IconToolbarPlus.tooltip = "Add a item with this list.";

        m_IconToolbarMinus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus"));
        m_IconToolbarMinus.tooltip = "Remove a Item in this list.";

        changed = false;
        Undo.RecordObject(target, "assetruleundo");
    }

    void OnDisable()
    {
        if (changed)
        {
            EditorUtility.SetDirty(target);

            if (EditorUtility.DisplayDialog("Unsaved Settings", "Unsaved AssetRule Changes", "Apply", "Revert"))
            {
                Apply((AssetRule)target);
            }
            else
            {
                Undo.PerformUndo();
                //SerializedObject so = new SerializedObject(target);
                //so.SetIsDifferentCacheDirty();
                //so.Update();
            }
        }
        changed = false;
    }
}