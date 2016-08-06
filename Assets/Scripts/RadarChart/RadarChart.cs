using UnityEngine.UI;
using UnityEngine;

[ExecuteInEditMode]
public class RadarChart : Graphic
{
    public float m_AngleOffset;

    /// <summary>
    /// 百分比
    /// </summary>
    [Range(0, 1)]
    [SerializeField]
    private float[] m_Percents;

    private Rect m_Rect;
    private int m_Cnt;

    public void Reset(float[] p)
    {
        m_Percents = p;
    }

    public void Change(int idx, float p)
    {
        if (m_Percents == null || idx < 0 || idx >= m_Percents.Length)
        {
            return;
        }
        m_Percents[idx] = p;
    }

    private Vector2 GetPoint(int idx)
    {
        Vector2 ret = Vector2.zero;
        float angle = 360f / m_Cnt * idx + m_AngleOffset;
        ret.x = m_Rect.width * Mathf.Cos(angle * Mathf.Deg2Rad);
        ret.y = m_Rect.height * Mathf.Sin(angle * Mathf.Deg2Rad);
        ret *= m_Percents[idx] * 0.5f;
        ret += m_Rect.center;
        return ret;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (m_Percents == null || m_Percents.Length < 3)
        {
            return;
        }

        m_Rect = GetPixelAdjustedRect();
        m_Cnt = m_Percents.Length;

        for (int i = 0; i < m_Cnt; i++)
        {
            vh.AddVert(GetPoint(i), color, Vector2.zero);
        }
        vh.AddVert(m_Rect.center, color, Vector2.zero);

        for (int i = 0; i < m_Cnt; i++)
        {
            vh.AddTriangle(m_Cnt, i, (i + 1) % m_Cnt);
        }
    }
}