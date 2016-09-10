using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DrawGizmos : MonoBehaviour
{
    public bool m_IsDraw = true;

    public bool m_IsDrawCube = true;
    public float m_CubeSize = 0.01f;
    public Color m_CubeColor = Color.blue;

    public bool m_IsDrawDir = false;
    public float m_DirSize = 0.5f;

#if UNITY_EDITOR
    public bool m_IsDrawLabel = true;
    public Color m_LabelColor = Color.blue;
#endif

    void Start()
    {

    }

    void Update()
    {

    }

    void OnDrawGizmos()
    {
        if (m_IsDraw == false)
        {
            return;
        }

        if (m_IsDrawCube)
        {
            Gizmos.color = m_CubeColor;
            Gizmos.DrawWireCube(this.transform.position, Vector3.one * m_CubeSize);
            Gizmos.color = Color.white;
        }

#if UNITY_EDITOR
        if (m_IsDrawLabel)
        {
            GUI.color = m_LabelColor;
            Handles.Label(this.transform.position, this.transform.gameObject.name);
            GUI.color = Color.white;
        }
#endif

        if (m_IsDrawDir)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * m_DirSize);
            Gizmos.color = Color.white;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.up * m_DirSize);
            Gizmos.color = Color.white;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.right * m_DirSize);
            Gizmos.color = Color.white;
        }
    }
}
