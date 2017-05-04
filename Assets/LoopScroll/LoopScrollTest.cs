using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoopScrollTest : MonoBehaviour
{
    private Transform m_Prefab;

    private LoopScroll m_LoopScroll;

    void Awake()
    {
        m_Prefab = this.transform.FindChild("Prefab");
        m_Prefab.gameObject.SetActive(false);

        m_LoopScroll = this.gameObject.AddComponent<LoopScroll>();
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
        m_LoopScroll.Reset();

        m_LoopScroll.m_Center = true;
        m_LoopScroll.m_Loop = true;
        m_LoopScroll.m_Height = 50;
        m_LoopScroll.m_Width = 200;

        m_LoopScroll.m_Dir = LoopScroll.Dir.Vertical;
        m_LoopScroll.m_Prefab = m_Prefab.gameObject;

        List<int> data = new List<int>();
        for (int i = 0; i < 50; i++)
        {
            data.Add(i * i);
        }

        m_LoopScroll.m_TotalCnt = data.Count;
        m_LoopScroll.m_RefreshRate = 2;
        
        m_LoopScroll.m_Spacing = new Vector2(2, 2);
        m_LoopScroll.m_StartIdx = 0;
        m_LoopScroll.m_ViewCnt = 3;
        m_LoopScroll.m_AddCnt = 1;

        m_LoopScroll.m_OnCenterItem = (item, idx) =>
        {
            Debug.LogError("centerItem " + idx + " " + data[idx]);
        };
        m_LoopScroll.m_OnFillItem = (item, idx) =>
        {
            Text tex = item.FindChild("Text").GetComponent<Text>();
            tex.text = data[idx].ToString();
            Debug.LogWarning("fillItem " + idx + " " + data[idx]);
        };

        m_LoopScroll.Init();
    }
}