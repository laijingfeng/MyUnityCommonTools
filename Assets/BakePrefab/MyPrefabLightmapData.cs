using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
#endif

public class MyPrefabLightmapData : MonoBehaviour
{
    [System.Serializable]
    struct RendererInfo
    {
        public MeshRenderer renderer;
        public int lightmapIndex;
        public Vector4 lightmapOffsetScale;
    }

    [SerializeField]
    RendererInfo[] m_RendererInfo;
    [SerializeField]
    Texture2D[] m_LightmapFars;
    [SerializeField]
    Texture2D[] m_LightmapNears;

    void Awake()
    {
        AddLightmap();
    }

    void Start()
    {

    }

    private void AddLightmap()
    {
        if (this.m_RendererInfo == null || this.m_RendererInfo.Length <= 0)
        {
            return;
        }

        List<LightmapData> datas = new List<LightmapData>(LightmapSettings.lightmaps);
        int[] idxs = new int[m_LightmapFars.Length];
        
        for (int i = 0, imax = m_LightmapFars.Length; i < imax; i++)
        {
            int idx = FindIdx(datas, m_LightmapFars[i]);
            if (idx == -1)
            {
                idxs[i] = datas.Count;

                LightmapData newData = new LightmapData();
                newData.lightmapFar = m_LightmapFars[i];
                newData.lightmapNear = m_LightmapNears[i];

                datas.Add(newData);
            }
            else
            {
                idxs[i] = idx;
            }
        }


        ApplyRendererInfo(m_RendererInfo, idxs);

        if (datas.Count > LightmapSettings.lightmaps.Length)
        {
            LightmapSettings.lightmaps = datas.ToArray();
        }
    }

    private int FindIdx(List<LightmapData> datas, Texture2D tex)
    {
        int ret = -1;
        for (int i = 0, imax = datas.Count; i < imax; i++)
        {
            if (tex.Equals(datas[i].lightmapFar))
            {
                ret = i;
                break;
            }
        }
        return ret;
    }

    private void ApplyRendererInfo(RendererInfo[] infos, int[] idxs)
    {
        for (int i = 0; i < infos.Length; i++)
        {
            var info = infos[i];
            info.renderer.lightmapIndex = idxs[info.lightmapIndex];
            info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;
        }
    }

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Assets/BakePrefab")]
    public static void Bake()
    {
        UnityEditor.Lightmapping.giWorkflowMode = UnityEditor.Lightmapping.GIWorkflowMode.OnDemand;
        UnityEditor.Lightmapping.Bake();

        CopyLightmap();

        foreach (MyPrefabLightmapData obj in UnityEngine.Object.FindObjectsOfType(typeof(MyPrefabLightmapData)))
        {
            if (obj != null)
            {
                obj.Bind();
            }
        }
    }

    private static void CopyLightmap()
    {
        UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
        string assetPath = Path.GetDirectoryName(s.path) + "/" + s.name;
        string fullPath = Application.dataPath + "/../" + assetPath;

        if (Directory.Exists(fullPath) == false)
        {
            return;
        }

        string[] files = Directory.GetFiles(fullPath);
        string fileAssetPath;
        foreach (string file in files)
        {
            if (file.Contains(".meta") == true)
            {
                continue;
            }
            if (file.Contains(".exr") == false)
            {
                continue;
            }
            fileAssetPath = file.Substring(file.LastIndexOf("Assets"));

            AssetDatabase.ImportAsset(fileAssetPath, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(fileAssetPath) as TextureImporter;
            importer.isReadable = true;
            AssetDatabase.ImportAsset(fileAssetPath, ImportAssetOptions.ForceUpdate);

            Texture2D assetLightmap = AssetDatabase.LoadAssetAtPath<Texture2D>(fileAssetPath);

            string newDir = Path.GetDirectoryName(s.path);
            //不能创建.exr的资源
            string newAssetPath = newDir + "/" + Path.GetFileNameWithoutExtension(fileAssetPath) + ".asset";
            Texture2D newLightmap = Instantiate<Texture2D>(assetLightmap);

            AssetDatabase.CreateAsset(newLightmap, newAssetPath);

            importer.isReadable = false;
            AssetDatabase.ImportAsset(fileAssetPath, ImportAssetOptions.ForceUpdate);
        }
    }

    private Texture2D GetLightmapAsset(int idx, bool far, bool fromCopyFile = true)
    {
        UnityEngine.SceneManagement.Scene s = EditorSceneManager.GetActiveScene();
        string assetPath = Path.GetDirectoryName(s.path);
        if (fromCopyFile == false)
        {
            assetPath = assetPath + "/" + s.name;
        }
        string fullPath = Application.dataPath + "/../" + assetPath;
        string fileName = string.Format("Lightmap-{0}_comp_{1}.{2}", idx, far ? "light" : "dir", fromCopyFile ? "asset" : "exr");

        if (File.Exists(fullPath + "/" + fileName) == false)
        {
            return null;
        }

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + "/" + fileName);
        return tex;
    }

    /// <summary>
    /// 绑定数据
    /// </summary>
    public void Bind()
    {
        List<Texture2D> lightmapFars = new List<Texture2D>();
        List<Texture2D> lightmapNears = new List<Texture2D>();
        List<RendererInfo> rendererInfos = new List<RendererInfo>();

        MeshRenderer[] rds = this.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer rd in rds)
        {
            //排除没烘焙的物体，如非静态的
            if (rd.lightmapIndex != -1)
            {
                RendererInfo info = new RendererInfo();
                info.lightmapOffsetScale = rd.lightmapScaleOffset;
                info.renderer = rd;

                Texture2D lightmapFar = GetLightmapAsset(rd.lightmapIndex, true);
                Texture2D lightmapNear = GetLightmapAsset(rd.lightmapIndex, false);

                info.lightmapIndex = lightmapFars.IndexOf(lightmapFar);
                if (info.lightmapIndex == -1)
                {
                    info.lightmapIndex = lightmapFars.Count;
                    lightmapFars.Add(lightmapFar);
                    lightmapNears.Add(lightmapNear);
                }

                rendererInfos.Add(info);
            }
        }
        this.m_LightmapFars = lightmapFars.ToArray();
        this.m_LightmapNears = lightmapNears.ToArray();
        this.m_RendererInfo = rendererInfos.ToArray();
    }

#endif
}