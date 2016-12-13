using UnityEngine;
using Jerry;
using UnityEngine.EventSystems;

public class UGUIEventListenerTest : MonoBehaviour
{
    public GameObject m_Cube;
    public GameObject m_Image;
    public GameObject m_Button;

    void Awake()
    {
        UserEventMgr.AddEvent("Test1", EventTest1);
    }

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
            UGUIEventListener.Get(m_Button.gameObject, null, true).onClick += OnClickGo;
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
            if (EventSystem.current != null
                && EventSystem.current.currentSelectedGameObject != null)
            {
                Debug.LogWarning("current selected Go : " + EventSystem.current.currentSelectedGameObject.name);
            }
            else
            {
                Debug.LogWarning("current selected Go : null");
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            UserEventMgr.DispatchEvent("Test1", new object[] { true });
        }
    }

    void OnDestroy()
    {
        UserEventMgr.RemoveEvent("Test1", EventTest1);
    }

    private void EventTest1(object[] args)
    {
        if (args == null
            || args.Length < 1)
        {
            Debug.Log("UGUITest no args");
        }
        else
        {
            bool hi = (bool)args[0];
            Debug.Log("UGUITest hi = " + hi);
        }
    }
}