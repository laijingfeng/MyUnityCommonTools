using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WayPointMgr
{
    #region 变量

    /// <summary>
    /// 路点列表
    /// </summary>
    private List<Vector3> m_WayPointList;

    /// <summary>
    /// 路点数量
    /// </summary>
    private int m_NumPoints;

    /// <summary>
    /// 到原点的距离
    /// </summary>
    private List<float> m_Dis;

    /// <summary>
    /// 总长度
    /// </summary>
    private float m_Length = 0f;

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

    private int p0n;
    private int p1n;
    private int p2n;
    private int p3n;

    private float i;
    private Vector3 P0;
    private Vector3 P1;
    private Vector3 P2;
    private Vector3 P3;

    #endregion 变量

    public WayPointMgr()
    {
        m_WayPointList = new List<Vector3>();
        m_Dis = new List<float>();
        m_Length = 0;
        m_NumPoints = 0;
    }

    private void Reset()
    {
        m_NumPoints = m_WayPointList.Count;
        m_Length = 0;

        m_Dis.Clear();
        m_Dis.Add(0);
        for (int i = 1; i < m_NumPoints; ++i)
        {
            m_Length += (m_WayPointList[i] - m_WayPointList[i - 1]).magnitude;
            m_Dis.Add(m_Length);
        }
    }

    #region 对外接口

    /// <summary>
    /// 设置路点
    /// </summary>
    /// <param name="points"></param>
    public void SetWayPoints(List<Vector3> points)
    {
        m_WayPointList.Clear();
        m_Length = 0f;
        foreach (Vector3 t in points)
        {
            m_WayPointList.Add(t);
        }
        Reset();
    }

    /// <summary>
    /// 设置路点
    /// </summary>
    /// <param name="points"></param>
    public void SetWayPoints(List<Transform> points)
    {
        List<Vector3> vPoints = new List<Vector3>();
        foreach (Transform t in points)
        {
            if (t != null)
            {
                vPoints.Add(t.position);
            }
        }
        SetWayPoints(vPoints);
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
    /// 获得路点位置
    /// </summary>
    /// <param name="dist">已经走过的距离</param>
    /// <returns></returns>
    public Vector3 GetRoutePosition(float dist, bool smooth = false)
    {
        int point = 0;
        while (m_Dis[point] < dist)
        {
            if (point >= m_NumPoints - 1)
            {
                break;
            }
            ++point;
        }

        p1n = ((point - 1) + m_NumPoints) % m_NumPoints;
        p2n = point;

        i = Mathf.InverseLerp(m_Dis[p1n], m_Dis[p2n], dist);

        if (smooth)
        {
            p0n = ((point - 2) + m_NumPoints) % m_NumPoints;
            p3n = (point + 1) % m_NumPoints;

            P0 = m_WayPointList[p0n];
            P1 = m_WayPointList[p1n];
            P2 = m_WayPointList[p2n];
            P3 = m_WayPointList[p3n];

            return CatmullRom(P0, P1, P2, P3, i);
        }
        else
        {
            p1n = ((point - 1) + m_NumPoints) % m_NumPoints;
            p2n = point;

            return Vector3.Lerp(m_WayPointList[p1n], m_WayPointList[p2n], i);
        }
    }

    /// <summary>
    /// <para>索引为idx的点的距离</para>
    /// <para>模拟非闭合曲线时，第0个点会有些异常，可以从第1个点开始</para>
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public float GetDisOfIdx(int idx)
    {
        if (idx < 0 || idx >= m_Dis.Count)
        {
            return 0;
        }
        return m_Dis[idx];
    }

    #endregion 对外接口

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

    #region 调试

    public void DrawPath(bool smooth, float smoothSteps = 100)
    {
        if(m_NumPoints <= 0)
        {
            return;
        }

        Gizmos.color = Color.green;

        Vector3 prev = m_WayPointList[0];
        if (smooth)
        {
            for (float dist = 0; dist <= Length + 0.001f; dist += Length / smoothSteps)
            {
                Vector3 next = GetRoutePosition(dist, smooth);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
        else
        {
            for (int i = 1, imax = m_WayPointList.Count; i < imax; i++)
            {
                Vector3 next = m_WayPointList[i];
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }

    #endregion 调试
}
