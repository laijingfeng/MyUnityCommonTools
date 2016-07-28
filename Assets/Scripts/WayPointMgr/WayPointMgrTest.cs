using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public class WayPointMgrTest : MonoBehaviour
{
    public bool m_DrawGizmos = false;
    public bool m_Smooth = false;
    public Transform m_PointFather;
    public string m_Points;
    public float m_Speed = 0.01f;

    private WayPointMgr m_WayMgr;
    private float m_PassedDis;
    private bool m_Moving = false;

#if UNITY_EDITOR
    private int m_Step = 1;
#endif

    void Start()
    {
        m_WayMgr = new WayPointMgr();

        int[] arr = Util.StringToIntArray(m_Points, ',');
        List<Vector3> list = new List<Vector3>();

        Transform tf;
        foreach (int id in arr)
        {
            tf = m_PointFather.FindChild(string.Format("p{0}", id));
            if (tf != null)
            {
                list.Add(tf.position);
            }
        }

        m_WayMgr.SetWayPoints(list.ToArray());
        m_PassedDis = 0f;
        m_Moving = true;
#if UNITY_EDITOR
        m_Step = (int)(m_WayMgr.Length / m_Speed);
#endif
    }

    void Update()
    {
        UpdateMove();
    }

    /// <summary>
    /// 更新移动
    /// </summary>
    private void UpdateMove()
    {
        if (m_Moving == false || m_WayMgr == null)
        {
            return;
        }

        m_PassedDis = Mathf.Min(m_PassedDis + m_Speed, m_WayMgr.Length);
        WayPointMgr.RoutePoint po = m_WayMgr.GetRoutePoint(m_PassedDis, m_Smooth);
        this.transform.position = po.position;
        if (po.direction != Vector3.zero)//zero会有一个Unity日志
        {
            this.transform.rotation = Quaternion.LookRotation(po.direction);
        }

        if (m_PassedDis >= m_WayMgr.Length)
        {
            m_Moving = false;
            Finish();
        }
    }

    /// <summary>
    /// 结束
    /// </summary>
    private void Finish()
    {
        GameObject.Destroy(this.gameObject);
    }

    /// <summary>
    /// 绘制路
    /// </summary>
    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (m_DrawGizmos == false)
        {
            return;
        }

        if (m_WayMgr != null)
        {
            m_WayMgr.DrawPath(m_Smooth, m_Step);
        }
#endif
    }
}
