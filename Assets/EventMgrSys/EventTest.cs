using UnityEngine;
using System.Collections;
using Jerry;

public class EventTest : MonoBehaviour
{
    void Awake()
    {
        UserEventMgr.AddEvent("Test1", EventTest1);
    }

    void Start()
    {
    }

    void Update()
    {
    }

    void OnDestroy()
    {
        UserEventMgr.RemoveEvent("Test1", EventTest1);
    }

    private void EventTest1(object[] args)
    {
        if(args == null
            || args.Length < 1)
        {
            Debug.Log("EventTest no args");
        }
        else
        {
            bool hi = (bool)args[0];
            Debug.Log("EventTest hi = " + hi);
        }
    }
}
