using UnityEngine;

namespace Jerry
{
    public class FPS : MonoBehaviour
    {
        public bool m_ShowFPS = true;
        public bool m_ShowMemory = true;

        void Start()
        {
            m_Frames = 0;
            m_TimeLeft = m_UpdateInterval;
        }

        void Update()
        {
            UpdateMemory();
            UpdateFPS();
        }

        void OnGUI()
        {
            if (m_ShowFPS || m_ShowMemory)
            {
                GUILayout.BeginVertical("box");
                if (m_ShowMemory)
                {
                    GUILayout.Label(sUserMemory);
                }
                if (m_ShowFPS)
                {
                    if (m_CurFps > 60)
                    {
                        GUILayout.Label(string.Format("FPS:<color=green>{0}</color>", ((int)m_CurFps).ToString("D3")));
                    }
                    else if (m_CurFps < 30)
                    {
                        GUILayout.Label(string.Format("FPS:<color=red>{0}</color>", ((int)m_CurFps).ToString("D3")));
                    }
                    else
                    {
                        GUILayout.Label(string.Format("FPS:<color=yellow>{0}</color>", ((int)m_CurFps).ToString("D3")));
                    }
                }
                GUILayout.EndVertical();
            }
        }

        #region Memory

        private string sUserMemory;

        private uint MonoUsedM;
        private uint AllMemory;

        private void UpdateMemory()
        {
            if (m_ShowMemory == false)
            {
                return;
            }

            sUserMemory = "";
            MonoUsedM = Profiler.GetMonoUsedSize() / 1000000;
            AllMemory = Profiler.GetTotalAllocatedMemory() / 1000000;

            sUserMemory += string.Format("MonoUsed:{0}M\n", MonoUsedM);
            sUserMemory += string.Format("AllMemory:{0}M\n", AllMemory);
            sUserMemory += string.Format("UnUsedReserved:{0}M\n", Profiler.GetTotalUnusedReservedMemory() / 1000000);
            sUserMemory += string.Format("MonoHeap:{0}K\n", Profiler.GetMonoHeapSize() / 1000);
            sUserMemory += string.Format("MonoUsed:{0}K\n", Profiler.GetMonoUsedSize() / 1000);
            sUserMemory += string.Format("Allocated:{0}K\n", Profiler.GetTotalAllocatedMemory() / 1000);
            sUserMemory += string.Format("Reserved:{0}K\n", Profiler.GetTotalReservedMemory() / 1000);
            sUserMemory += string.Format("UnusedReserved:{0}K\n", Profiler.GetTotalUnusedReservedMemory() / 1000);
            sUserMemory += string.Format("UsedHeap:{0}K", Profiler.usedHeapSize / 1000);
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
            if (m_ShowFPS == false)
            {
                return;
            }

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
    }
}