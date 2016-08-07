using UnityEngine;

public class FPS : MonoBehaviour
{
    void Start()
    {
        m_Frames = 0;
        m_TimeLeft = m_UpdateInterval;
    }

    void Update()
    {
        UpdateUsed();
        UpdateFPS();
    }

    public bool m_Show = true;

    #region Memory

    private string sUserMemory;

    private string s;
    
    private uint MonoUsedM;
    private uint AllMemory;

    void UpdateUsed()
    {
        sUserMemory = "";
        MonoUsedM = Profiler.GetMonoUsedSize() / 1000000;
        AllMemory = Profiler.GetTotalAllocatedMemory() / 1000000;

        sUserMemory += "MonoUsed:" + MonoUsedM + "M" + "\n";
        sUserMemory += "AllMemory:" + AllMemory + "M" + "\n";
        sUserMemory += "UnUsedReserved:" + Profiler.GetTotalUnusedReservedMemory() / 1000000 + "M" + "\n";

        s = "";
        s += " MonoHeap:" + Profiler.GetMonoHeapSize() / 1000 + "k";
        s += " MonoUsed:" + Profiler.GetMonoUsedSize() / 1000 + "k";
        s += " Allocated:" + Profiler.GetTotalAllocatedMemory() / 1000 + "k";
        s += " Reserved:" + Profiler.GetTotalReservedMemory() / 1000 + "k";
        s += " UnusedReserved:" + Profiler.GetTotalUnusedReservedMemory() / 1000 + "k";
        s += " UsedHeap:" + Profiler.usedHeapSize / 1000 + "k";
    }

    #endregion Memory

    #region FPS

    /// <summary>
    /// 更新频率
    /// </summary>
    private float m_UpdateInterval = 0.5f;

    private float m_Frames = 0;

    /// <summary>
    /// 剩余时间
    /// </summary>
    private float m_TimeLeft;

    private float m_CurFps;
    
    void UpdateFPS()
    {
        m_TimeLeft -= Time.deltaTime;
        
        ++m_Frames;

        if (m_TimeLeft <= 0.0f)
        {
            m_CurFps = m_Frames / (m_UpdateInterval - m_TimeLeft);
            m_TimeLeft = m_UpdateInterval;
            m_Frames = 0;
        }
    }

    #endregion FPS

    void OnGUI()
    {
        if (m_Show)
        {
            GUI.color = new Color(1, 0, 0);
            GUI.Label(new Rect(10, 10, 200, 60), sUserMemory);
            GUI.Label(new Rect(10, 60, 100, 30), "FPS:" + m_CurFps.ToString("f2"));
        }
    }
}