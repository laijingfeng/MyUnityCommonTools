using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WayPointMgrTest : MonoBehaviour
{
    public bool m_DrawGizmos = false;
    public bool m_Smooth = false;
    public List<Transform> ways = new List<Transform>();
    public float m_Speed = 0.01f;

    private WayPointMgr m_WayMgr;
    private float m_PassedDis;
    private bool m_Moving = false;
    
    void Start()
    {
        m_WayMgr = new WayPointMgr();
        m_WayMgr.SetWayPoints(ways.ToArray());
        m_PassedDis = 0f;
        m_Moving = true;
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
        this.transform.rotation = Quaternion.LookRotation(po.direction);

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
        if (m_DrawGizmos == false)
        {
            return;
        }

        if (m_WayMgr != null)
        {
            m_WayMgr.DrawPath(m_Smooth, 100);
        }
    }
}
