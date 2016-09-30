using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Jerry;
using UnityEngine.EventSystems;

public class UGUIEventListenerTest : MonoBehaviour
{
    public GameObject m_Cube;
    public GameObject m_Image;
    public GameObject m_Button;

    void Start()
    {
        if (m_Cube != null)
        {
            UGUIEventListener.Get(m_Cube, new object[] { 1, "cube" }).onClick += OnClickGo;
        }

        if (m_Image != null)
        {
            UGUIEventListener.Get(m_Image.gameObject, new object[] { 10, "image" }, false).onClick += OnClickGo;
        }

        if (m_Button != null)
        {
            UGUIEventListener.Get(m_Button.gameObject, null, false).onClick += OnClickGo;
        }
    }

    private void OnClickGo(GameObject go)
    {
        Debug.Log("click " + go.name);

        object[] data = UGUIEventListener.GetData(go);
        if (data != null)
        {
            foreach (object obj in data)
            {
                Debug.Log("--" + obj.ToString());
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                Debug.LogWarning("current selected Go : null");
            }
            else
            {
                Debug.LogWarning("current selected Go : " + EventSystem.current.currentSelectedGameObject.name);
            }
        }
    }
}
