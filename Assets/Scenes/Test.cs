using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
    void Start()
    {
        JerryDebug.LogWarning("Start " + Input.touchSupported);
        JerryDebug.CtrAction1Name = "Click";
        JerryDebug.CtrAction1 += CtrAction1;
    }

    private void CtrAction1()
    {
        JerryDebug.LogWarning("Click");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            JerryDebug.LogWarning("mouse_down:" + Input.mousePosition);
        }
    }
}
