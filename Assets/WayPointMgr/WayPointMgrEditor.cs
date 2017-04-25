#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;
using Jerry;

[ExecuteInEditMode]
public class WayPointMgrEditor : MonoBehaviour
{
    /// <summary>
    /// 绘制Gizmos
    /// </summary>
    public bool m_DrawGizmos = false;

    public Transform m_WayFather;

    public bool HI__ShowCreatePoints = false;

    [ConditionalHide("HI__ShowCreatePoints")]
    public Transform m_PointPrefab;

    [ConditionalHide("HI__ShowCreatePoints")]
    public int m_NameStart = 0;

    [ConditionalHide("HI__ShowCreatePoints")]
    public int m_NameAdd = 1;

    [ConditionalHide("HI__ShowCreatePoints")]
    public int m_CreateCnt = 1;

    [ConditionalHide("HI__ShowCreatePoints")]
    public Vector3 m_Offset;

    [ConditionalHide("HI__ShowCreatePoints")]
    [Tooltip("根据上面的设置创建路点")]
    public bool m_CreatePos;

    public bool HI__ShowRenamePoints = false;

    [ConditionalHide("HI__ShowRenamePoints")]
    public int m_NameFrom = 0;

    /// <summary>
    /// 升序重命名
    /// </summary>
    [ConditionalHide("HI__ShowRenamePoints")]
    [Tooltip("对WayFather下的路点重命名")]
    public bool m_RenameASC;

    [ConditionalHide("HI__ShowRenamePoints")]
    public bool m_ResortByName = false;

    public bool HI__ShowDrawWays = false;

    /// <summary>
    /// 是否光滑
    /// </summary>
    [SerializeField]
    [ConditionalHide("HI__ShowDrawWays")]
    private bool smoothRoute = true;

    /// <summary>
    /// <para>编辑器中路径划分的步数</para>
    /// </summary>
    [ConditionalHide("HI__ShowDrawWays")]
    public int editorVisualisationSubsteps = 100;

    /// <summary>
    /// 要绘制的路径
    /// </summary>
    [ConditionalHide("HI__ShowDrawWays")]
    [Tooltip("要绘制的路径，英文逗号隔开")]
    public string m_DrawStr;

    /// <summary>
    /// 实时绘制
    /// </summary>
    [ConditionalHide("HI__ShowDrawWays")]
    public bool m_DrawRealTime = false;

    /// <summary>
    /// 重置路
    /// </summary>
    [ConditionalHide("HI__ShowDrawWays")]
    [Tooltip("绘制路径，根据WayFather和DrawStr")]
    public bool m_DrawWay = false;

    private WayPointMgr m_Mgr = new WayPointMgr();

    private List<Transform> m_NodeList = new List<Transform>();

    private void Update()
    {
        editorVisualisationSubsteps = Mathf.Clamp(editorVisualisationSubsteps, 1, 500);

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

        int[] arr = JerryUtil.String2TArray<int>(m_DrawStr, ',');
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

#endif
