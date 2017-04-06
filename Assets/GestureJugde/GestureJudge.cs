using UnityEngine;
using System.Collections;
using Jerry;

public class GestureJudge : MonoBehaviour
{
    private string m_Info = "touch to start";

    public float m_MinSwipeDistance = 0.1f;
    
    /// <summary>
    /// 正方向可以偏差的判断角度
    /// </summary>
    [Range(5, 45)]
    public float m_Angle = 45;

    public enum GestureDir
    {
        Right = 0,
        Down,
        Left,
        Up,
    }

    /// <summary>
    /// 已经开始滑动
    /// </summary>
    private bool m_TouchStarted;

    /// <summary>
    /// 开始点击的位置
    /// </summary>
    private Vector2 m_TouchStartPos;

    /// <summary>
    /// 开始的时间
    /// </summary>
    //private float touchStartTime;

    /// <summary>
    /// 最小像素距离
    /// </summary>
    private float m_MinSwipeDistancePixels;

    /// <summary>
    /// 是否移动设备
    /// </summary>
    private bool m_IsPhone;

    void Start()
    {
        float screenDiagonalSize = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
        m_MinSwipeDistancePixels = m_MinSwipeDistance * screenDiagonalSize;

#if UNITY_EDITOR
        m_IsPhone = false;
#else
#if UNITY_ANDROID || UNITY_IPHONE
        m_IsPhone = true;
#else
        m_IsPhone = false;
#endif
#endif
    }

    void Update()
    {
        if (m_IsPhone)
        {
            UpdatePhone();
        }
        else
        {
            UpdatePC();
        }
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(m_Info);
        GUILayout.EndHorizontal();
    }

    private void UpdatePC()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_TouchStarted = true;
            m_TouchStartPos = Vector3.zero;//JerryUtil.GetClickPos();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (m_TouchStarted)
            {
                Judge();
                m_TouchStarted = false;
            }
        }
    }

    private void UpdatePhone()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.touches[0];

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        m_TouchStarted = true;
                        m_TouchStartPos = touch.position;
                        //m_TouchStartTime = Time.realtimeSinceStartup;
                    }
                    break;
                case TouchPhase.Ended:
                    {
                        if (m_TouchStarted)
                        {
                            Judge();
                            m_TouchStarted = false;
                        }
                    }
                    break;
                case TouchPhase.Canceled:
                    {
                        m_TouchStarted = false;
                    }
                    break;
                case TouchPhase.Stationary:
                    {
                        //if (m_TouchStarted)
                        //{
                        //    m_TouchStartPos = touch.position;
                        //    m_TouchStartTime = Time.realtimeSinceStartup;
                        //}
                    }
                    break;
                case TouchPhase.Moved:
                    {

                    }
                    break;
            }
        }
    }

    private void Judge()
    {
        Vector3 start = m_TouchStartPos;
        Vector3 end = Vector3.zero;//JerryUtil.GetClickPos();

        float dis = Vector2.Distance(start, end);

        if (dis < m_MinSwipeDistancePixels)
        {
            return;
        }

        float dy = end.y - start.y;
        float dx = end.x - start.x;

        //结果是：上0，右90，左-90
        float angle = Mathf.Rad2Deg * Mathf.Atan2(dx, dy);

        angle = (360 + angle - 45) % 360;//正右是45度

        if (angle >= 45 - m_Angle && angle <= 45 + m_Angle)
        {
            GestureEvent(GestureDir.Right);
        }
        else if (angle >= 135 - m_Angle && angle <= 135 + m_Angle)
        {
            GestureEvent(GestureDir.Down);
        }
        else if (angle >= 225 - m_Angle && angle <= 225 + m_Angle)
        {
            GestureEvent(GestureDir.Left);
        }
        else if (angle >= 315 - m_Angle && angle <= 315 + m_Angle)
        {
            GestureEvent(GestureDir.Up);
        }
        else
        {
            m_Info = "touch false";
        }
    }

    private void GestureEvent(GestureDir dir)
    {
        m_Info = "touch ok:" + dir.ToString();
    }
}
