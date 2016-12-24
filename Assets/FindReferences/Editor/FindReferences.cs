//#define HavePrefabInPrefab

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

namespace Jerry
{
    /// <summary>
    /// 查找引用
    /// </summary>
    public class FindReferences : EditorWindow
    {
        #region 变量

        /// <summary>
        /// 配置
        /// </summary>
        [SerializeField]
        private static Config m_Config;
        /// <summary>
        /// 当前正在找的类型
        /// </summary>
        private static FindType m_FindType = FindType.none;
        /// <summary>
        /// 当前正在找的文件路径
        /// </summary>
        private static string m_FindPath;
        /// <summary>
        /// 当前正在找的文件名，不带后缀
        /// </summary>
        private static string m_FindName;
        /// <summary>
        /// 当前的关联类型表
        /// </summary>
        private static List<RelateType> m_RelateType = new List<RelateType>();
        /// <summary>
        /// 要查找详情的对象，在m_RelatedGo查找m_FindName
        /// </summary>
        private static GameObject m_RelatedGo;

        #endregion 变量

        #region 入口

        /// <summary>
        /// 设置，路径
        /// </summary>
        [MenuItem("Jerry/FindReferences/Find Setting", false)]
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

        /// <summary>
        /// 查找引用
        /// </summary>
        [MenuItem("Assets/Find References", false)]
        private static void Find()
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;

            if (CullingFind(true) == false)
            {
                return;
            }

            DoFind();
        }

        [MenuItem("Assets/Find References", true)]
        private static bool VFind()
        {
            return CullingFind();
        }

        /// <summary>
        /// 进一步查找细节
        /// </summary>
        [MenuItem("Assets/Find Detail", false)]
        private static void FindDetail()
        {
            if (CullingDetail(true, false) == false)
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

        [MenuItem("GameObject/Jerry/Find Detail", false, 10)]
        private static void FindDetail2()
        {
            if (CullingDetail(true, true) == false)
            {
                return;
            }
            DoFindDetail();
        }

        [MenuItem("GameObject/Jerry/Find Detail", true, 10)]
        private static bool VFindDetail2()
        {
            return CullingDetail(false, true);
        }

        #endregion 入口

        #region 查找实现

        private static void DoFind()
        {
            m_RelateType.Clear();
            switch (m_FindType)
            {
                case FindType.cs:
                case FindType.mat:
                case FindType.prefab:
                case FindType.ttf:
                    {
                        m_RelateType.AddRange(new RelateType[] { RelateType.prefab, RelateType.unity });
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

            //Debug.Log(guid + " " + m_Config.Path);
            //foreach (RelateType rt in m_RelateType)
            //{
            //    Debug.Log("x " + rt.ToString());
            //}

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
                default:
                    {
#if HavePrefabInPrefab
                    PrefabInPrefab[] coms = m_RelatedGo.GetComponentsInChildren<PrefabInPrefab>(true);
                    foreach (PrefabInPrefab com in coms)
                    {
                        if (com.PrefabName().Equals(m_FindName))
                        {
                            Debug.LogWarning(GetRelativeAssetsPath(com.transform), com);
                        }
                    }
#else
                        Debug.LogWarning("类型[" + m_FindType.ToString() + "]暂不支持FindDetail");
#endif
                    }
                    break;
            }
        }

        private static bool CullingDetail(bool work = false, bool canEmpty = false)
        {
            if (m_FindType == FindType.none)
            {
                return false;
            }

            UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.GameObject), SelectionMode.Assets);
            if (objs == null || objs.Length != 1)
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

        /// <summary>
        /// 过滤查找引用
        /// </summary>
        /// <param name="work">是否是实际查找</param>
        /// <returns></returns>
        private static bool CullingFind(bool work = false)
        {
            UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            if (objs == null || objs.Length != 1)
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

            //Debug.Log(findName + " " + extension);

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

        #endregion 查找实现

        #region 辅助

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

        #endregion 辅助

        #region 结构

        [System.Serializable]
        public class Config
        {
            public string Path;
            public Config()
            {
                Path = "Assets/";
            }
        }

        /// <summary>
        /// 支持的查找类型，对应是文件后缀
        /// </summary>
        public enum FindType
        {
            /// <summary>
            /// none是标空
            /// </summary>
            none = 0,
            prefab,
            mat,
            unity,
            asset,
            cs,
            ttf,
        }

        /// <summary>
        /// 关联类型，将到哪些类型的文件去找目标
        /// </summary>
        public enum RelateType
        {
            prefab,
            unity,
        }

        #endregion 结构
    }
}