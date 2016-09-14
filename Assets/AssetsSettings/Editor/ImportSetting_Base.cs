using UnityEngine;
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
    public string m_MyName;

    private string m_path;

    public virtual void Draw()
    {
        EditorGUILayout.BeginVertical("box");

        this.m_InUse = EditorGUILayout.Toggle("InUse", this.m_InUse);

        EditorGUILayout.LabelField("FilterType", this.m_TypeFilter.ToString());

        this.m_MyName = EditorGUILayout.TextField("Name", this.m_MyName);

        this.m_PathFilterType = (PathFilterType)EditorGUILayout.EnumPopup("PathFilterType", this.m_PathFilterType);

        EditorGUILayout.LabelField("PathFilter");
        this.m_PathFilter = EditorGUILayout.TextArea(this.m_PathFilter, GUILayout.MinWidth(180));
        EditorGUILayout.HelpBox("&:与\n|:或\n!:非\n_name0&(!hi|cc)\n(路径含_name0)且((路径不含hi)或(路径含cc))", MessageType.Info, true);

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

    /// <summary>
    /// 路径匹配
    /// </summary>
    /// <param name="path"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    private bool CheckPath(string path, string filter)
    {
        m_path = path;

        if (string.IsNullOrEmpty(filter) || string.IsNullOrEmpty(m_path))
        {
            return true;
        }

        //换行符号去掉
        //空格不能去掉，空格可能是资源命名的空格
        filter = filter.Replace("\n", "");

        if (GrammarPass(filter) == false)
        {
            return false;
        }

        return DoSearch(filter);
    }

    private static List<char> m_oks = new List<char>() 
    {
        '&','|','_','(',')',' ','!'
    };

    /// <summary>
    /// 语法检测
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
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
        List<string> ret = GetOneLogicPiece(filter);

        //一个逻辑语句，下面两种情况
        //1.带符号:a&b
        //2.不带符号:a
        if (ret == null || (ret.Count != 1 && ret.Count != 3))
        {
            UnityEngine.Debug.LogError("filter error:" + filter);
            return false;
        }

        //debug
        //foreach (string s in ret)
        //{
        //    Debug.LogWarning(filter + ":" + s);
        //}

        if (ret.Count == 1)
        {
            return JudgeOne(ret[0]);
        }

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
        if (s.Length <= 1)
        {
            return false;
        }

        if (s[0] == '!')
        {
            return !m_path.Contains(s.Substring(1));
        }
        else
        {
            return m_path.Contains(s);
        }
    }

    /// <summary>
    /// 获取一个逻辑语句
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    private List<string> GetOneLogicPiece(string filter)
    {
        List<string> ret = new List<string>();
        if (string.IsNullOrEmpty(filter))
        {
            return ret;
        }
        char[] chs = filter.ToCharArray();
        int cnt = 0;
        int s = 0;//开始下标
        int e = chs.Length - 1;//结束下标

        //去除最外层的括号
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
        m_MyName = name;
        m_PathFilterType = PathFilterType.Name;
    }

    /// <param name="importer"></param>
    /// <returns></returns>
    public virtual bool ApplySettings(AssetImporter importer)
    {
        return true;
    }
}