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
    [Range(3, 500)]
    public float editorVisualisationSubsteps = 100;

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
    
    /// <summary>
    /// 路点列表
    /// </summary>
    private List<Vector3> m_WayPointList = new List<Vector3>();

    /// <summary>
    /// 路点数量
    /// </summary>
    private int numPoints;

    /// <summary>
    /// 到原点的距离
    /// </summary>
    private float[] distances;

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

        SetWayPoints(list);
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

    private void Reset()
    {
        if (m_WayPointList.Count > 1)
        {
            CacheDistances();
        }
        numPoints = m_WayPointList.Count;
    }

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
    /// 获得路点位置
    /// </summary>
    /// <param name="dist">已经走过的距离</param>
    /// <returns></returns>
    public Vector3 GetRoutePosition(float dist, bool smooth = false)
    {
        int point = 0;
        while (distances[point] < dist)
        {
            if (point >= numPoints - 1)
            {
                break;
            }
            ++point;
        }

        p1n = ((point - 1) + numPoints) % numPoints;
        p2n = point;

        i = Mathf.InverseLerp(distances[p1n], distances[p2n], dist);

        if (smooth)
        {
            p0n = ((point - 2) + numPoints) % numPoints;
            p3n = (point + 1) % numPoints;

            P0 = m_WayPointList[p0n];
            P1 = m_WayPointList[p1n];
            P2 = m_WayPointList[p2n];
            P3 = m_WayPointList[p3n];

            return CatmullRom(P0, P1, P2, P3, i);
        }
        else
        {
            p1n = ((point - 1) + numPoints) % numPoints;
            p2n = point;

            return Vector3.Lerp(m_WayPointList[p1n], m_WayPointList[p2n], i);
        }
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
    {
        return 0.5f *
               ((2 * p1) + (-p0 + p2) * i + (2 * p0 - 5 * p1 + 4 * p2 - p3) * i * i +
                (-p0 + 3 * p1 - 3 * p2 + p3) * i * i * i);
    }

    private void CacheDistances()
    {
        distances = new float[m_WayPointList.Count];
        distances[0] = 0f;
        for (int i = 1, imax = m_WayPointList.Count; i < imax; ++i)
        {
            distances[i] = distances[i - 1] + (m_WayPointList[i] - m_WayPointList[i - 1]).magnitude;
        }
        m_Length = distances[distances.Length - 1];
    }

    private void OnDrawGizmos()
    {
        DrawGizmos();
    }

    /// <summary>
    /// 绘制
    /// </summary>
    private void DrawGizmos()
    {
        if (m_DrawGizmos == false)
        {
            return;
        }

        if (m_WayPointList.Count > 1)
        {
            numPoints = m_WayPointList.Count;

            CacheDistances();

            Gizmos.color = Color.yellow;

            Vector3 prev = m_WayPointList[0];
            if (smoothRoute)
            {
                for (float dist = 0; dist <= Length + 0.001f; dist += Length / editorVisualisationSubsteps)
                {
                    Vector3 next = GetRoutePosition(dist, smoothRoute);
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
