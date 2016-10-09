using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Gradient : BaseMeshEffect
{
    [SerializeField]
    private Color32 topColor = Color.white;
    [SerializeField]
    private Color32 bottomColor = Color.black;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }

        int count = vh.currentVertCount;
        if (count == 0)
        {
            return;
        }

        List<UIVertex> vertexs = new List<UIVertex>();
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = new UIVertex();
            vh.PopulateUIVertex(ref vertex, i);
            vertexs.Add(vertex);
        }

        float topY = vertexs[0].position.y;
        float bottomY = vertexs[0].position.y;

        for (int i = 1; i < count; i++)
        {
            float y = vertexs[i].position.y;
            if (y > topY)
            {
                topY = y;
            }
            else if (y < bottomY)
            {
                bottomY = y;
            }
        }

        float height = topY - bottomY;
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = vertexs[i];
            Color32 color = Color32.Lerp(bottomColor, topColor, (vertex.position.y - bottomY) / height);
            vertex.color = color;
            vh.SetUIVertex(vertex, i);
        }
    }
}