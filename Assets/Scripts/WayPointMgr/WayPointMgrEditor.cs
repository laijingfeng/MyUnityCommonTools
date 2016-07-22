using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class WayPointMgrEditor : MonoBehaviour
{
    /// <summary>
    /// 绘制Gizmos
    /// </summary>
    public bool m_DrawGizmos = false;

    /// <summary>
    /// 是否光滑
    /// </summary>
    [SerializeField]
    private bool smoothRoute = true;

    /// <summary>
    /// <para>编辑器中路径划分的步数</para>
    /// </summary>
    [Range(2, 500)]
    public int editorVisualisationSubsteps = 100;

    public bool _11111111111111111111111111;

    public Transform m_WayFather;

    public Transform m_PointPrefab;

    public int m_NameStart = 0;

    public int m_NameAdd = 1;

    public int m_CreateCnt = 1;

    public float m_XOffset;

    public float m_ZOffset;

    [Tooltip("根据上面的设置创建路点")]
    public bool m_CreatePos;

    public bool _22222222222222222222222222;

    /// <summary>
    /// 升序重命名
    /// </summary>
    [Tooltip("对WayFather下的路点重命名")]
    public bool m_RenameASC;

    public bool _33333333333333333333333333;

    /// <summary>
    /// 要绘制的路径
    /// </summary>
    [Tooltip("要绘制的路径，英文逗号隔开")]
    public string m_DrawStr;

    /// <summary>
    /// 重置路
    /// </summary>
    [Tooltip("绘制路径，根据WayFather和DrawStr")]
    public bool m_DrawWay = false;

    private WayPointMgr m_Mgr = new WayPointMgr();

    private void Update()
    {
        if (m_DrawWay == true)
        {
            DrawPoint();
            m_DrawWay = false;
        }

        if (m_CreatePos == true)
        {
            m_CreatePos = false;
            CreatePos();
        }

        if (m_RenameASC)
        {
            m_RenameASC = false;
            RenameASC();
        }
    }

    private void DrawPoint()
    {
        if (m_WayFather == null)
        {
            Debug.LogError("WayFather is null");
            return;
        }

        int[] arr = StringToIntArray(m_DrawStr, ',');
        List<Vector3> list = new List<Vector3>();

        Transform tf;
        foreach (int id in arr)
        {
            tf = m_WayFather.FindChild(string.Format("p{0}", id));
            if (tf != null)
            {
                list.Add(tf.position);
            }
        }
        m_Mgr.SetWayPoints(list.ToArray());
    }

    /// <summary>
    /// 重命名
    /// </summary>
    private void RenameASC()
    {
        if (m_WayFather == null)
        {
            Debug.LogError("WayFather is null");
            return;
        }

        Transform tf;
        for (int i = 0, imax = m_WayFather.childCount; i < imax; i++)
        {
            tf = m_WayFather.GetChild(i);
            if (tf != null)
            {
                tf.name = string.Format("p{0}", i);
            }
        }
    }

    /// <summary>
    /// 创建路点
    /// </summary>
    private void CreatePos()
    {
        if (m_PointPrefab == null || m_WayFather == null)
        {
            Debug.LogError("WayFather is null");
            return;
        }

        Transform tOld;
        GameObject nGo;
        Vector3 vTmp;
        for (int i = 0; i < m_CreateCnt; i++)
        {
            tOld = m_WayFather.FindChild(string.Format("p{0}", m_NameStart + i * m_NameAdd));
            if (tOld != null)
            {
                GameObject.DestroyImmediate(tOld.gameObject);
            }
            nGo = GameObject.Instantiate(m_PointPrefab.gameObject);
            nGo.transform.parent = m_PointPrefab.parent;
            nGo.name = string.Format("p{0}", m_NameStart + i * m_NameAdd);
            nGo.transform.localScale = Vector3.one;
            nGo.transform.localRotation = Quaternion.Euler(Vector3.zero);
            vTmp = m_PointPrefab.localPosition;
            vTmp += new Vector3(m_XOffset, 0f, m_ZOffset) * (i + 1);
            nGo.transform.localPosition = vTmp;
        }
    }

    private void OnDrawGizmos()
    {
        if (m_DrawGizmos)
        {
            m_Mgr.DrawPath(smoothRoute, editorVisualisationSubsteps);
        }
    }

    #region 工具

    /// <summary>
    /// <para>StringToIntArray</para>
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public int[] StringToIntArray(string str, char separator = ',')
    {
        List<int> list = new List<int>();

        if (string.IsNullOrEmpty(str))
        {
            return list.ToArray();
        }

        int tmp;

        string[] str_array = str.Split(separator);
        foreach (string s in str_array)
        {
            if (int.TryParse(s, out tmp) == false)
            {
                return list.ToArray();
            }
            list.Add(tmp);
        }

        return list.ToArray();
    }

    #endregion 工具
}
