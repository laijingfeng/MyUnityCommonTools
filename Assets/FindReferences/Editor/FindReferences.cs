using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

public class FindReferences : EditorWindow
{
    [MenuItem("Window/Find Setting", false)]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindReferences));
        LoadSet();
    }

    void OnGUI()
    {
        m_Config.Path = EditorGUILayout.TextField("Path", m_Config.Path);
        if (GUILayout.Button("Reset"))
        {
            m_Config = new Config();
        }
        if (GUILayout.Button("Save"))
        {
            SaveSet();
        }
    }

    [SerializeField]
    private static Config m_Config;

    [System.Serializable]
    public class Config
    {
        public string Path;
        public Config()
        {
            Path = "Assets/";
        }
    }

    public enum FindType
    {
        none,
        prefab,
        mat,
        unity,
        asset,
        cs,
        ttf,
    }

    public enum RelateType
    {
        prefab,
        unity,
    }

    /// <summary>
    /// 当前正在找的类型
    /// </summary>
    private static FindType m_FindType = FindType.none;
    private static List<RelateType> m_RelateType = new List<RelateType>();
    private static string m_FindPath;
    private static string m_FindName;

    private static GameObject m_RelatedGo;

    private static void LoadSet()
    {
        string data = PlayerPrefs.GetString("FindRefrences_Set", string.Empty);
        if (!string.IsNullOrEmpty(data))
        {
            m_Config = JsonUtility.FromJson<Config>(data);
        }
        else
        {
            m_Config = new Config();
        }
    }

    private static void SaveSet()
    {
        if (m_Config.Path.StartsWith("Assets/") == false)
        {
            m_Config.Path = "Assets/";
        }
        PlayerPrefs.SetString("FindRefrences_Set", JsonUtility.ToJson(m_Config));
    }

    [MenuItem("GameObject/Jerry/Find Detail", true, 10)]
    private static bool VFindDetail2()
    {
        return CullingDetail(false, true);
    }

    [MenuItem("GameObject/Jerry/Find Detail", false, 10)]
    private static void FindDetail2()
    {
        if (CullingDetail(true, true) == false)
        {
            return;
        }
        DoFindDetail();
    }

    [MenuItem("Assets/Find Detail", true)]
    private static bool VFindDetail()
    {
        return CullingDetail();
    }

    [MenuItem("Assets/Find Detail", false)]
    private static void FindDetail()
    {
        if (CullingDetail(true, false) == false)
        {
            return;
        }
        DoFindDetail();
    }

    private static void DoFindDetail()
    {
        switch (m_FindType)
        {
            case FindType.ttf:
                {
                    Text[] ts = m_RelatedGo.GetComponentsInChildren<Text>(true);
                    foreach (Text t in ts)
                    {
                        if (t.font.name.Equals(m_FindName))
                        {
                            Debug.LogWarning(GetRelativeAssetsPath(t.transform), t);
                        }
                    }
                }
                break;
            case FindType.cs:
                {
                    Component[] coms = m_RelatedGo.GetComponentsInChildren<Component>(true);
                    foreach (Component com in coms)
                    {
                        if (com.GetType().ToString().Equals(m_FindName))
                        {
                            Debug.LogWarning(GetRelativeAssetsPath(com.transform), com);
                        }
                    }
                }
                break;
        }
    }

    [MenuItem("Assets/Find References", false)]
    private static void Find()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;

        if (Culling(true) == false)
        {
            return;
        }

        m_RelateType.Clear();
        switch (m_FindType)
        {
            case FindType.cs:
            case FindType.mat:
            case FindType.ttf:
                {
                    m_RelateType.AddRange(new RelateType[] { RelateType.prefab, RelateType.unity });
                }
                break;
            case FindType.prefab:
                {
                    m_RelateType.AddRange(new RelateType[] { RelateType.unity });
                }
                break;
        }

        LoadSet();
        if (m_Config.Path.StartsWith("Assets/") == false)
        {
            Debug.LogError("Path error");
            return;
        }

        Debug.Log(string.Format("Find:{0}", m_FindPath));
        //Debug.Log(string.Format("Find:{0} type={1}", m_FindPath, m_FindType));

        string guid = AssetDatabase.AssetPathToGUID(m_FindPath);
        string[] files = Directory.GetFiles(Application.dataPath + "/../" + m_Config.Path, "*.*", SearchOption.AllDirectories)
            .Where(s =>
                m_RelateType.FindIndex(x =>
                    string.Format(".{0}", x.ToString()).Equals(Path.GetExtension(s).ToLower())
                ) >= 0
            ).ToArray();

        if (files == null || files.Length <= 0)
        {
            return;
        }

        int startIndex = 0;
        EditorApplication.update = delegate()
        {
            string file = files[startIndex];

            bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

            if (Regex.IsMatch(File.ReadAllText(file), guid))
            {
                Debug.Log(GetRelativeAssetsPath(file), AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetsPath(file)));
            }

            startIndex++;
            if (isCancel || startIndex >= files.Length)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                startIndex = 0;
                Debug.Log("匹配结束");
            }
        };
    }

    [MenuItem("Assets/Find References", true)]
    private static bool VFind()
    {
        return Culling();
    }

    private static bool CullingDetail(bool work = false, bool canEmpty = false)
    {
        if (m_FindType == FindType.none)
        {
            return false;
        }

        UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.GameObject), SelectionMode.Assets);
        if (objs == null || objs.Length <= 0)
        {
            return false;
        }

        string relatedPath = AssetDatabase.GetAssetPath(objs[0]);
        if (string.IsNullOrEmpty(relatedPath))
        {
            if (canEmpty)
            {
                if (work)
                {
                    m_RelatedGo = objs[0] as GameObject;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        LoadSet();

        if (relatedPath.Contains(m_Config.Path) == false)
        {
            return false;
        }

        string extension = Path.GetExtension(relatedPath);
        if (string.IsNullOrEmpty(extension))
        {
            return false;
        }

        int idx = m_RelateType.FindIndex((x) => string.Format(".{0}", x.ToString()).Equals(extension.ToLower()));
        if (idx >= 0)
        {
            if (work)
            {
                m_RelatedGo = objs[0] as GameObject;
            }

            return true;
        }

        return false;
    }

    private static bool Culling(bool work = false)
    {
        UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        if (objs == null || objs.Length <= 0)
        {
            return false;
        }

        string findPath = AssetDatabase.GetAssetPath(objs[0]);
        if (string.IsNullOrEmpty(findPath))
        {
            return false;
        }

        string findName = Path.GetFileNameWithoutExtension(findPath);

        string extension = Path.GetExtension(findPath);
        if (string.IsNullOrEmpty(extension))
        {
            return false;
        }

        Array arr = Enum.GetValues(typeof(FindType));
        foreach (FindType val in arr)
        {
            if (extension.ToLower().Equals(string.Format(".{0}", val.ToString())))
            {
                if (work)
                {
                    m_FindPath = findPath;
                    m_FindName = findName;
                    m_FindType = val;
                }
                return true;
            }
        }

        return false;
    }

    private static string GetRelativeAssetsPath(Transform tf)
    {
        if (tf == null)
        {
            return string.Empty;
        }

        string ret = tf.name;
        while (tf.parent != null)
        {
            tf = tf.parent;
            ret = tf.name + "/" + ret;
        }
        return ret;
    }

    private static string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }
}