using UnityEngine.UI;
using UnityEngine;

public class RadarChart : Graphic
{
    [Range(0f, 360f)]
    public float m_AngleOffset;

    public float m_LineWidth = 1f;
    public Color m_LineColor = Color.black;
    public bool m_DrawLine = false;

    public float m_BoundLineWidth = 1f;
    public Color m_BoundLineColor = Color.black;
    public bool m_DrawBoundLine = false;

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
        SetAllDirty();
    }

    public void Change(int idx, float p)
    {
        if (m_Percents == null || idx < 0 || idx >= m_Percents.Length)
        {
            return;
        }
        m_Percents[idx] = p;
        SetAllDirty();
    }

    private Vector2 GetPoint(int idx, bool full = true)
    {
        Vector2 ret = Vector2.zero;
        float angle = 360f / m_Cnt * idx + m_AngleOffset;
        ret.x = 0.5f * m_Rect.width * Mathf.Cos(angle * Mathf.Deg2Rad);
        ret.y = 0.5f * m_Rect.height * Mathf.Sin(angle * Mathf.Deg2Rad);
        ret += m_Rect.center;
        if (!full)
        {
            ret *= m_Percents[idx];
        }
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
            vh.AddVert(GetPoint(i, false), color, Vector2.zero);
        }
        vh.AddVert(Vector2.zero, color, Vector2.zero);

        for (int i = 0; i < m_Cnt; i++)
        {
            vh.AddTriangle(m_Cnt, i, (i + 1) % m_Cnt);
        }

        if (m_DrawLine)
        {
            for (int i = 0; i < m_Cnt; i++)
            {
                vh.AddUIVertexQuad(GetLine(Vector2.zero, GetPoint(i, true), m_LineWidth, m_LineColor));
            }
        }

        if (m_DrawBoundLine)
        {
            for (int i = 0; i < m_Cnt; i++)
            {
                vh.AddUIVertexQuad(GetLine(GetPoint(i, false), GetPoint((i + 1) % m_Cnt, false), m_BoundLineWidth, m_BoundLineColor));
            }
        }
    }

    private UIVertex[] GetLine(Vector2 s, Vector2 e, float width, Color color)
    {
        UIVertex[] vers = new UIVertex[4];
        Vector2 v1 = e - s;
        Vector2 v2 = (v1.y == 0f) ? new Vector2(0f, 1f) : new Vector2(1f, -v1.x / v1.y);
        v2.Normalize();
        v2 *= 0.5f * width;
        for (int i = 0; i < 4; i++)
        {
            vers[i] = UIVertex.simpleVert;
            vers[i].color = color;
            vers[i].uv0 = Vector2.zero;
            if (i == 0)
            {
                vers[i].position = s + v2;
            }
            else if (i == 1)
            {
                vers[i].position = e + v2;
            }
            else if (i == 2)
            {
                vers[i].position = e - v2;
            }
            else if (i == 3)
            {
                vers[i].position = s - v2;
            }
        }
        return vers;
    }
}