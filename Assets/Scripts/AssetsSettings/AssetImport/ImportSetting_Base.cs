using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class ImportSetting_Base : ScriptableObject
{
    public enum FilterType
    {
        MODEL,
        TEXTURE,
    }

    public enum PathFilterType
    {
        Name,
        Path,
    }

    public bool m_InUse;
    public FilterType m_TypeFilter;
    public PathFilterType m_PathFilterType;
    public string m_PathFilter;
    public string m_Name;

    private string m_path;

    public virtual void Draw()
    {
        EditorGUILayout.BeginVertical("box");

        this.m_InUse = EditorGUILayout.Toggle("InUse", this.m_InUse);

        EditorGUILayout.LabelField("FilterType", this.m_TypeFilter.ToString());

        this.m_Name = EditorGUILayout.TextField("Name", this.m_Name);

        this.m_PathFilterType = (PathFilterType)EditorGUILayout.EnumPopup("PathFilterType", this.m_PathFilterType);

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

        return CheckPath((m_PathFilterType == PathFilterType.Path) ? importer.assetPath : Path.GetFileName(importer.assetPath), m_PathFilter);
    }

    #region CheckPath

    private bool CheckPath(string path, string filter)
    {
        m_path = path;

        if (string.IsNullOrEmpty(filter) || string.IsNullOrEmpty(m_path))
        {
            return true;
        }

        filter = filter.Replace("\n", "");

        if (GrammarPass(filter) == false)
        {
            return false;
        }

        return DoSearch(filter);
    }

    private static List<char> m_oks = new List<char>() 
    {
        '=','&','|','_','(',')',' '
    };

    private bool GrammarPass(string filter)
    {
        if (string.IsNullOrEmpty(filter))
        {
            return true;
        }
        char[] chs = filter.ToCharArray();
        for (int i = 0; i < chs.Length; i++)
        {
            if (chs[i] >= '0' && chs[i] <= '9')
            {
                continue;
            }
            if (chs[i] >= 'a' && chs[i] <= 'z')
            {
                continue;
            }
            if (chs[i] >= 'A' && chs[i] <= 'Z')
            {
                continue;
            }
            if (m_oks.Contains(chs[i]) == false)
            {
                return false;
            }
        }
        return true;
    }

    private bool DoSearch(string filter)
    {
        List<string> ret = GetPieces(filter);
        if (ret == null || (ret.Count != 1 && ret.Count != 3))
        {
            UnityEngine.Debug.LogError("filter error:" + filter);
            return false;
        }

        if (ret.Count == 1)
        {
            return JudgeOne(ret[0]);
        }

        //debug
        //foreach (string s in ret)
        //{
        //    Debug.LogWarning(filter + ":" + s);
        //}

        if (ret[1] == "&")
        {
            return DoSearch(ret[0]) && DoSearch(ret[2]);
        }
        else
        {
            return DoSearch(ret[0]) || DoSearch(ret[2]);
        }
    }

    private bool JudgeOne(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            UnityEngine.Debug.LogError("filter error:" + s);
            return false;
        }
        s = s.Replace("(", "").Replace(")", "");
        string[] ss = s.Split('=');
        if (ss == null || ss.Length != 2)
        {
            UnityEngine.Debug.LogError("filter error:" + s);
            return false;
        }
        if (ss[1] == "t")
        {
            return (m_path.Contains(ss[0]) == true);
        }
        else if (ss[1] == "f")
        {
            return (m_path.Contains(ss[0]) == false);
        }
        else
        {
            UnityEngine.Debug.LogError("filter error:" + s);
            return false;
        }
    }

    private List<string> GetPieces(string filter)
    {
        List<string> ret = new List<string>();
        if (string.IsNullOrEmpty(filter))
        {
            return ret;
        }
        char[] chs = filter.ToCharArray();
        int cnt = 0;
        int s = 0;
        int e = chs.Length - 1;

        while (true)
        {
            if (s >= e)
            {
                break;
            }

            if (chs[s] == '(' && chs[e] == ')')
            {
                s++;
                e--;
            }
            else
            {
                break;
            }
        }

        int idx = s;

        for (int i = s; i <= e; i++)
        {
            if (chs[i] == '(')
            {
                cnt++;
            }
            else if (chs[i] == ')')
            {
                cnt--;
                if (cnt < 0)
                {
                    UnityEngine.Debug.LogError("filter error");
                    return null;
                }
            }
            else if (chs[i] == '&')
            {
                if (cnt == 0)
                {
                    ret.Add(filter.Substring(idx, i - idx));
                    idx = i + 1;
                    ret.Add("&");
                    break;
                }
            }
            else if (chs[i] == '|')
            {
                if (cnt == 0)
                {
                    ret.Add(filter.Substring(idx, i - idx));
                    idx = i + 1;
                    ret.Add("|");
                    break;
                }
            }
        }

        ret.Add(filter.Substring(idx, e - idx + 1));

        return ret;
    }

    #endregion CheckPath

    public virtual void Init(string name)
    {
        m_InUse = true;
        m_PathFilter = string.Empty;
        m_Name = name;
        m_PathFilterType = PathFilterType.Name;
    }

    /// <param name="importer"></param>
    /// <returns></returns>
    public virtual bool ApplySettings(AssetImporter importer)
    {
        return true;
    }
}
