using UnityEngine.UI;
using UnityEngine;

[ExecuteInEditMode]
public class RadarChart : Graphic
{
    [Range(3, 10)]
    [SerializeField]
    private int m_SideCnt = 3;

    [SerializeField]
    private float m_StartDegree;

#if UNITY_EDITOR
    [SerializeField]
    private bool m_DoSet = false;

    [SerializeField]
    private bool m_RealTime = false;
#endif

    [SerializeField]
    private float m_Radius;

    /// <summary>
    /// 百分比
    /// </summary>
    [Range(0, 1)]
    [SerializeField]
    private float[] m_Percents;

    /// <summary>
    /// 顶点
    /// </summary>
    private Vector3[] m_Vertexes;

    private float[] m_SideDegree;

    /// <summary>
    /// 是否需要重画
    /// </summary>
    private bool m_IsDirty = false;

    private void DoData()
    {
        m_Percents = new float[m_SideCnt];

        m_Vertexes = new Vector3[m_SideCnt + 1];

        float ave = 360f / m_SideCnt;
        m_SideDegree = new float[m_SideCnt];

        m_Vertexes[m_SideCnt] = this.transform.localPosition;
        for (int i = 0; i < m_SideCnt; i++)
        {
            m_SideDegree[i] = m_StartDegree + ave * i;
            m_Vertexes[i] = m_Vertexes[m_SideCnt] + (CalVecByDegree(m_SideDegree[i]) - m_Vertexes[m_SideCnt]) * m_Percents[i];
        }

#if UNITY_EDITOR
        m_IsDirty = true;
#endif
    }

    public void NewData(int sideCnt, float radius, float[] p, float startDegree = 0)
    {
        if (p == null || p.Length != sideCnt)
        {
            return;
        }

        DoData();

        for (int i = 0; i < m_SideCnt; i++)
        {
            m_Percents[i] = p[i];
        }

        m_IsDirty = true;
    }

    private Vector3 CalVecByDegree(float degree)
    {
        Vector3 ret = Vector3.zero;
        ret.x = m_Radius * Mathf.Cos(degree * Mathf.Deg2Rad);
        ret.y = m_Radius * Mathf.Sin(degree * Mathf.Deg2Rad);
        ret += this.transform.localPosition;
        return ret;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (m_RealTime)
        {
            m_IsDirty = true;
        }
        else
        {
            m_IsDirty = false;
        }

        if (m_DoSet)
        {
            m_DoSet = false;
            DoData();
        }
#endif
        if (m_IsDirty)
        {
            m_IsDirty = false;
            Refresh();
        }
    }

    public void Refresh()
    {
        if (m_Percents == null || m_Percents.Length != m_SideCnt)
        {
            return;
        }

        m_Vertexes[m_SideCnt] = this.transform.localPosition;
        for (int i = 0; i < m_SideCnt; i++)
        {
            m_Vertexes[i] = m_Vertexes[m_SideCnt] + (CalVecByDegree(m_SideDegree[i]) - m_Vertexes[m_SideCnt]) * m_Percents[i];
        }

        SetAllDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (m_Percents == null || m_Percents.Length != m_SideCnt)
        {
            return;
        }

        vh.Clear();

        for (int i = 0; i < m_SideCnt + 1; i++)
        {
            vh.AddVert(m_Vertexes[i], color, Vector2.zero);
        }

        for (int i = 0; i < m_SideCnt; i++)
        {
            vh.AddTriangle(m_SideCnt, i, (i + 1) % m_SideCnt);
        }
    }
}