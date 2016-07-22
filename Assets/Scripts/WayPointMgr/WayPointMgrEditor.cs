using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

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

    public Transform m_WayFather;

    public bool _11111111111111111111111111;

    public Transform m_PointPrefab;

    public int m_NameStart = 0;

    public int m_NameAdd = 1;

    public int m_CreateCnt = 1;

    public Vector3 m_Offset;

    [Tooltip("根据上面的设置创建路点")]
    public bool m_CreatePos;

    public bool _22222222222222222222222222;

    public int m_NameFrom = 0;

    /// <summary>
    /// 升序重命名
    /// </summary>
    [Tooltip("对WayFather下的路点重命名")]
    public bool m_RenameASC;

    public bool _33333333333333333333333333;

    public bool m_ResortByName = false;

    public bool _44444444444444444444444444;

    /// <summary>
    /// 要绘制的路径
    /// </summary>
    [Tooltip("要绘制的路径，英文逗号隔开")]
    public string m_DrawStr;

    /// <summary>
    /// 实时绘制
    /// </summary>
    public bool m_DrawRealTime = false;

    /// <summary>
    /// 重置路
    /// </summary>
    [Tooltip("绘制路径，根据WayFather和DrawStr")]
    public bool m_DrawWay = false;

    private WayPointMgr m_Mgr = new WayPointMgr();

    private List<Transform> m_NodeList = new List<Transform>();

    private void Update()
    {
        if (m_DrawWay == true || m_DrawRealTime)
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

        if (m_ResortByName)
        {
            m_ResortByName = false;
            ResortByName();
        }
    }

    private void DrawPoint()
    {
        if (m_WayFather == null)
        {
            Debug.LogError("WayFather is null");
            return;
        }

        int[] arr = Util.StringToIntArray(m_DrawStr, ',');
        m_NodeList.Clear();
        List<Vector3> list = new List<Vector3>();

        Transform tf;
        foreach (int id in arr)
        {
            tf = m_WayFather.FindChild(string.Format("p{0}", id));
            if (tf != null)
            {
                m_NodeList.Add(tf);
                list.Add(tf.position);
            }
        }
        m_Mgr.SetWayPoints(m_NodeList.ToArray());
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
                tf.name = string.Format("p{0}", i + m_NameFrom);
            }
        }
    }

    private void ResortByName()
    {
        if (m_WayFather == null)
        {
            Debug.LogError("WayFather is null");
            return;
        }

        Transform tf;
        for (int i = 0; i < 100; i++)
        {
            tf = m_WayFather.FindChild(string.Format("p{0}", i));
            if (tf == null)
            {
                break;
            }
            tf.SetAsLastSibling();
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
            vTmp += m_Offset * (i + 1);
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
}
