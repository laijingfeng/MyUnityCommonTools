using Jerry;
using UnityEngine;

public abstract class GestureBase : MonoBehaviour
{
    /// <summary>
    /// 最小滑动对角线的百分比
    /// </summary>
    public float m_MinSwipeDistance = 0.1f;
    protected int m_CutPart = 4;
    /// <summary>
    /// 每一份的角度
    /// </summary>
    private int m_PartAngle = 0;

    /// <summary>
    /// 已经开始滑动
    /// </summary>
    private bool m_TouchStarted;

    /// <summary>
    /// 开始点击的位置
    /// </summary>
    private Vector2 m_TouchStartPos;

    /// <summary>
    /// 最小像素距离
    /// </summary>
    private float m_MinSwipeDistancePixels;

    /// <summary>
    /// 是否移动设备
    /// </summary>
    private bool m_IsPhone;

    protected virtual void Start()
    {
        float screenDiagonalSize = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
        m_MinSwipeDistancePixels = m_MinSwipeDistance * screenDiagonalSize;

        m_PartAngle = 360 / m_CutPart;

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

    private void UpdatePC()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_TouchStarted = true;
            m_TouchStartPos = JerryUtil.GetClickPos();
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
            Touch touch = Input.touches[0];
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        m_TouchStarted = true;
                        m_TouchStartPos = touch.position;
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
        Vector3 end = JerryUtil.GetClickPos();

        float dis = Vector2.Distance(start, end);

        if (dis < m_MinSwipeDistancePixels)
        {
            return;
        }

        float angle = Mathf.Rad2Deg * Mathf.Atan2(end.y - start.y, end.x - start.x);
        angle = (360 + angle) % 360;
        int iAngle = (int)((angle + m_PartAngle * 0.5f) % 360);

        //Debug.LogWarning("i:" + iAngle + " " + angle);

        JudgeDir(iAngle / m_PartAngle);
    }

    protected virtual void JudgeDir(int idx)
    {
        Debug.LogWarning(idx);
    }
}