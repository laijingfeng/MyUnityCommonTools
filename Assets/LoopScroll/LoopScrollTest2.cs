using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopScrollTest2 : MonoBehaviour
{
    private Transform m_Prefab;

    private LoopScroll2 m_LoopScroll;

    void Awake()
    {
        m_LoopScroll = this.GetComponent<LoopScroll2>();
    }

    void Start()
    {
        FillScroll();
    }

    void Update()
    {
    }

    private void FillScroll()
    {
    }
}