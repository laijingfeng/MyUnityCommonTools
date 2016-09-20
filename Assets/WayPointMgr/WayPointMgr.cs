using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// <para>备注：曲线时，点与点的距离要尽量均匀</para>
/// <para>CatmullRom(0,1,3,40,0.5)=-0.3</para>
/// </summary>
public class WayPointMgr
{
    #region 变量

    /// <summary>
    /// 路点列表
    /// </summary>
    private Vector3[] m_WayPointList;

    /// <summary>
    /// 路点数量
    /// </summary>
    private int m_NumPoints;

    /// <summary>
    /// 到原点的距离
    /// </summary>
    private float[] m_Dis;

    /// <summary>
    /// 总长度
    /// </summary>
    private float m_Length;

    private float i;

    /// <summary>
    /// 为了更好的优化，不要破坏pid的值
    /// </summary>
    private int pid;

    #endregion 变量

    public WayPointMgr()
    {
        m_WayPointList = null;
        m_Dis = null;
        m_Length = 0;
        m_NumPoints = 0;
        pid = 1;
    }

    private void Reset()
    {
        m_Length = 0;
        m_Dis = new float[m_NumPoints];
        m_Dis[0] = 0f;
        for (int i = 1; i < m_NumPoints; ++i)
        {
            m_Length += (m_WayPointList[i + 1] - m_WayPointList[i]).magnitude;
            m_Dis[i] = m_Length;
        }
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
    {
        return 0.5f *
               ((2 * p1) + (-p0 + p2) * i + (2 * p0 - 5 * p1 + 4 * p2 - p3) * i * i +
                (-p0 + 3 * p1 - 3 * p2 + p3) * i * i * i);
    }

    /// <summary>
    /// 路点
    /// </summary>
    public struct RoutePoint
    {
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// 方向
        /// </summary>
        public Vector3 direction;

        public RoutePoint(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }

    #region 对外接口

    /// <summary>
    /// 路径总长度
    /// </summary>
    public float Length
    {
        get
        {
            return m_Length;
        }
    }

    /// <summary>
    /// 设置路点
    /// </summary>
    /// <param name="points">路点</param>
    public void SetWayPoints(Vector3[] points)
    {
        if (points == null || points.Length < 2)
        {
            Debug.LogError("points is null, or less than 2 points");
            return;
        }

        m_NumPoints = points.Length;
        m_WayPointList = new Vector3[m_NumPoints + 2];

        Array.Copy(points, 0, m_WayPointList, 1, points.Length);
        if (points[0].Equals(points[m_NumPoints - 1]))
        {
            m_WayPointList[0] = points[m_NumPoints - 2];
            m_WayPointList[m_NumPoints + 1] = points[1];
        }
        else
        {
            m_WayPointList[0] = points[0] + (points[0] - points[1]);
            m_WayPointList[m_NumPoints + 1] = points[m_NumPoints - 1] + (points[m_NumPoints - 1] - points[m_NumPoints - 2]);
        }

        Reset();
    }

    /// <summary>
    /// 设置路点
    /// </summary>
    /// <param name="points">路点</param>
    public void SetWayPoints(Transform[] points)
    {
        Vector3[] po = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            po[i] = points[i].position;
        }
        SetWayPoints(po);
    }

    /// <summary>
    /// 获得路点
    /// </summary>
    /// <param name="dist">已经走过的距离</param>
    /// <returns></returns>
    public RoutePoint GetRoutePoint(float dist, bool smooth = false)
    {
        Vector3 p1 = GetRoutePosition(dist, smooth);
        Vector3 p2 = GetRoutePosition(dist + 0.01f, smooth);
        Vector3 delta = p2 - p1;
        return new RoutePoint(p1, delta.normalized);
    }

    /// <summary>
    /// 独立出来，期待做一些优化
    /// </summary>
    /// <param name="dist"></param>
    private void GetPid(float dist)
    {
        if (pid <= m_NumPoints - 1 && m_Dis[pid - 1] <= dist)
        {
            //keep the value
        }
        else
        {
            pid = 1;
        }

        //由于浮点原因，dist可能比Length大，为了限定下标，枚举只到(m_NumPoints-1)
        for (; pid < m_NumPoints - 1; pid++)
        {
            if (m_Dis[pid] >= dist)
            {
                break;
            }
        }
    }

    /// <summary>
    /// 获得路点位置
    /// </summary>
    /// <param name="dist">已经走过的距离</param>
    /// <returns></returns>
    public Vector3 GetRoutePosition(float dist, bool smooth = false)
    {
        GetPid(dist);
        i = Mathf.InverseLerp(m_Dis[pid - 1], m_Dis[pid], dist);

        if (smooth)
        {
            return CatmullRom(m_WayPointList[pid - 1], m_WayPointList[pid],
                m_WayPointList[pid + 1], m_WayPointList[pid + 2], i);
        }
        else
        {
            return Vector3.Lerp(m_WayPointList[pid], m_WayPointList[pid + 1], i);
        }
    }

    #endregion 对外接口

#if UNITY_EDITOR

    #region 调试

    private GUIStyle style = new GUIStyle();

    public void DrawPath(bool smooth, int smoothSteps = 100)
    {
        if (m_NumPoints < 2 || smoothSteps < 1)
        {
            return;
        }

        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        Gizmos.color = Color.green;

        Vector3 prev = m_WayPointList[1];
        Vector3 next;
        if (smooth)
        {
            int nextColorId = 1;
            float dis;
            for (int i = 1; i <= smoothSteps; i++)
            {
                dis = i * 1.0f / smoothSteps * m_Length;
                next = GetRoutePosition(dis, smooth);
                if (dis > m_Dis[nextColorId])
                {
                    if (nextColorId < m_NumPoints - 1)
                    {
                        nextColorId++;
                        if (Gizmos.color == Color.green)
                        {
                            Gizmos.color = Color.cyan;
                        }
                        else
                        {
                            Gizmos.color = Color.green;
                        }
                    }
                }
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
        else
        {
            for (int i = 2; i < m_NumPoints + 1; i++)
            {
                next = m_WayPointList[i];
                Gizmos.DrawLine(prev, next);
                prev = next;
                if (Gizmos.color == Color.green)
                {
                    Gizmos.color = Color.cyan;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
            }
        }

        Handles.Label(m_WayPointList[1], "Begin", style);
        Handles.Label(m_WayPointList[m_NumPoints], "End", style);
    }

    #endregion 调试

#endif
}
