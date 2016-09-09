﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class LoopScroll : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public enum Dir
    {
        Horizontal,
        Vertical,
    }

    public delegate void OnFillItem(Transform item, int realIndex);

    #region 对外变量

    public OnFillItem m_OnFillItem;
    public OnFillItem m_OnCenterItem;

    public Transform m_Content;
    public GameObject m_Prefab;

    public int m_Width = 100, m_Height = 100;
    public Vector2 m_Spacing = new Vector2(2, 2);

    public float m_ViewFCnt = 3f;

    /// <summary>
    /// 第一个显示的idx，[0,m_TotalCnt)
    /// </summary>
    public int m_StartIdx = 0;
    public Dir m_Dir = Dir.Horizontal;
    public bool m_Center = false;
    public bool m_Loop = false;

    public int m_TotalCnt = 100;

    /// <summary>
    /// 每几帧刷新一次
    /// </summary>
    public int m_RefreshRate = 2;

    public bool m_Debug = false;

    #endregion 对外变量

    /// <summary>
    /// 暂时写死
    /// </summary>
    private int m_AddCnt = 2;

    /// <summary>
    /// 一屏能看见的数量，若不是整数向上取整
    /// </summary>
    private int m_ViewCnt = 4;

    private ScrollRect m_ScrollRect;
    private RectTransform m_ScrollRectSize;

    private RectTransform m_ContentRect;

    private int m_FIdx = 0, m_LIdx = 0;
    private Vector3 m_PosF, m_PosL;

    private Transform m_HandleTf;

    private List<Transform> m_Child = new List<Transform>();

    private bool m_Inited = false;
    private bool m_Awaked = false;

    void Awake()
    {
        m_ContentRect = m_Content.GetComponent<RectTransform>();
        m_ContentRect.pivot = Vector2.one * 0.5f;
        m_ContentRect.anchorMin = Vector2.one * 0.5f;
        m_ContentRect.anchorMax = Vector2.one * 0.5f;

        m_ScrollRect = this.transform.GetComponent<ScrollRect>();

        m_ScrollRectSize = m_ScrollRect.GetComponent<RectTransform>();
        m_ScrollRectSize.pivot = Vector2.one * 0.5f;
        m_ScrollRectSize.anchorMin = Vector2.one * 0.5f;
        m_ScrollRectSize.anchorMax = Vector2.one * 0.5f;

        m_Awaked = true;
        DoInit();
    }

    void Start()
    {
#if UNITY_EDITOR
        if (m_Debug)
        {
            Init();
        }
#endif
    }

    #region 对外接口

    public void Reset()
    {
        m_Inited = false;
    }

    /// <summary>
    /// 开始初始化，在设置后配置后调用
    /// </summary>
    public void Init()
    {
        RectTransform PrefabRect = m_Prefab.GetComponent<RectTransform>();
        PrefabRect.pivot = Vector2.one * 0.5f;
        PrefabRect.anchorMin = Vector2.one * 0.5f;
        PrefabRect.anchorMax = Vector2.one * 0.5f;

        m_LastPos = 0f;
        m_Frame = 0;
        m_ViewCnt = Mathf.CeilToInt(m_ViewFCnt);

        m_Inited = true;

        DoInit();
    }

    /// <summary>
    /// 关闭的时候清理垃圾
    /// </summary>
    public void DoClean()
    {
        if (m_Center)
        {
            iTween.Stop(m_Content.gameObject);
            this.StopCoroutine("DoCenter");
        }

        m_ScrollRect.onValueChanged.RemoveListener(OnScrollChange);
        Util.DestroyAllChildren(m_Content);
        m_Child.Clear();
    }

    #endregion 对外接口

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_Center)
        {
            iTween.Stop(m_Content.gameObject);
            this.StopCoroutine("DoCenter");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_Center)
        {
            iTween.Stop(m_Content.gameObject);
            this.StopCoroutine("DoCenter");
            this.StartCoroutine("DoCenter");
        }
    }

    private void DoInit()
    {
        if (m_Inited == false
            || m_Awaked == false)
        {
            return;
        }

        if (m_ViewCnt % 2 == 0)
        {
            m_Center = false;
        }

        DoClean();

        if (m_Dir == Dir.Horizontal)
        {
            m_ScrollRect.horizontal = true;
            m_ScrollRect.vertical = false;
        }
        else
        {
            m_ScrollRect.horizontal = false;
            m_ScrollRect.vertical = true;
        }
        m_ScrollRect.content = m_ContentRect;

        if (m_Center)
        {
            m_Loop = true;
        }

        Vector3 firstPos = Vector3.zero;
        if (m_Dir == Dir.Horizontal)
        {
            firstPos.x = -Mathf.Floor(0.5f * (m_ViewCnt + m_AddCnt)) * (m_Width + m_Spacing.x);
        }
        else
        {
            firstPos.y = Mathf.Floor(0.5f * (m_ViewCnt + m_AddCnt)) * (m_Height + m_Spacing.y);
        }

        if (m_Center)
        {
            m_ContentRect.localPosition = Vector3.zero;
            m_FIdx = GetIdx(m_StartIdx + 1 - (m_ViewCnt + m_AddCnt) / 2, false);
        }
        else
        {
            if (m_Loop || m_StartIdx * 2 >= m_AddCnt)
            {
                m_FIdx = GetIdx(m_StartIdx, false);
            }
            else
            {
                m_FIdx = GetIdx(m_StartIdx + 1, false);
            }

            if (m_Dir == Dir.Horizontal)
            {
                int half = (m_ViewCnt + m_AddCnt) / 2;
                if (m_FIdx != m_StartIdx)
                {
                    half--;
                }

                float x = half * (m_Width + m_Spacing.x);
                if (m_ViewCnt % 2 != 0)
                {
                    x += 0.5f * m_Width;
                }

                m_ContentRect.localPosition = new Vector3(x - 0.5f * m_ScrollRectSize.sizeDelta.x, 0, 0);
            }
            else
            {
                int half = (m_ViewCnt + m_AddCnt) / 2;
                if (m_FIdx != m_StartIdx)
                {
                    half--;
                }

                float x = half * (m_Width + m_Spacing.x);
                if (m_ViewCnt % 2 != 0)
                {
                    x += 0.5f * m_Width;
                }

                float y = half * (m_Height + m_Spacing.y);
                if (m_ViewCnt % 2 != 0)
                {
                    y += 0.5f * m_Height;
                }

                m_ContentRect.localPosition = new Vector3(0, 0.5f * m_ScrollRectSize.sizeDelta.y - y, 0);
            }
        }

        for (int i = 0; i < m_ViewCnt + m_AddCnt; i++)
        {
            GameObject go = CloneGo();

            go.transform.localPosition = firstPos;
            m_Child.Add(go.transform);
            Fill(go.transform, GetIdx(m_FIdx + i + 1, false));

            if (m_Dir == Dir.Horizontal)
            {
                firstPos.x += (m_Width + m_Spacing.x);
            }
            else
            {
                firstPos.y -= (m_Height + m_Spacing.y);
            }
        }

        m_LIdx = GetIdx(m_FIdx + m_ViewCnt + m_AddCnt, false);

        ResetSize();

        m_ScrollRect.onValueChanged.AddListener(OnScrollChange);
    }

    private GameObject CloneGo()
    {
        GameObject go = GameObject.Instantiate(m_Prefab);
        go.transform.SetParent(m_Content);
        go.SetActive(true);
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);
        return go;
    }

    private int GetIdx(int idx, bool next)
    {
        if (next)
        {
            return (idx + 1 + m_TotalCnt) % m_TotalCnt;
        }
        else
        {
            return (idx - 1 + m_TotalCnt) % m_TotalCnt;
        }
    }

    private Vector3 GetPos(Transform tf)
    {
        if (tf == null)
        {
            return Vector3.zero;
        }
        return tf.localPosition + tf.parent.localPosition;
    }

    private bool NeedRefresh(bool left)
    {
        if (m_Dir == Dir.Horizontal)
        {
            if (left)
            {
                if (m_Loop == false && m_FIdx == 0)
                {
                    return false;
                }
                return (m_PosF.x > -m_ScrollRectSize.sizeDelta.x * 0.5f);
            }
            else
            {
                if (m_Loop == false && m_LIdx == m_TotalCnt - 1)
                {
                    return false;
                }
                return (m_PosL.x < m_ScrollRectSize.sizeDelta.x * 0.5f);
            }
        }
        else
        {
            if (left)
            {
                if (m_Loop == false && m_FIdx == 0)
                {
                    return false;
                }
                return (m_PosF.y < m_ScrollRectSize.sizeDelta.y * 0.5f);
            }
            else
            {
                if (m_Loop == false && m_LIdx == m_TotalCnt - 1)
                {
                    return false;
                }
                return (m_PosL.y > -m_ScrollRectSize.sizeDelta.y * 0.5f);
            }
        }
    }

    /// <summary>
    /// 减小刷新频率，当前帧
    /// </summary>
    private int m_Frame = 0;

    private void OnScrollChange(Vector2 vet)
    {
        if (m_Inited == false || m_Awaked == false)
        {
            return;
        }

        m_Frame = (m_Frame + 1) % m_RefreshRate;
        if (m_Frame % m_RefreshRate != 0)
        {
            return;
        }

        m_PosF = GetPos(m_Child[0]);
        m_PosL = GetPos(m_Child[m_ViewCnt + m_AddCnt - 1]);

        if (NeedRefresh(true))
        {
            m_HandleTf = m_Child[m_ViewCnt + m_AddCnt - 1];

            if (m_Dir == Dir.Horizontal)
            {
                m_HandleTf.localPosition = m_Child[0].localPosition - (new Vector3(m_Width + m_Spacing.x, 0, 0));
            }
            else
            {
                m_HandleTf.localPosition = m_Child[0].localPosition + (new Vector3(0, m_Height + m_Spacing.y, 0));
            }
            m_Child.RemoveAt(m_ViewCnt + m_AddCnt - 1);
            m_Child.Insert(0, m_HandleTf);

            m_FIdx = GetIdx(m_FIdx, false);
            m_LIdx = GetIdx(m_LIdx, false);
            Fill(m_HandleTf, m_FIdx);

            ResetSize();
        }
        else if (NeedRefresh(false))
        {
            m_HandleTf = m_Child[0];
            if (m_Dir == Dir.Horizontal)
            {
                m_HandleTf.localPosition = m_Child[m_ViewCnt + m_AddCnt - 1].localPosition + (new Vector3(m_Width + m_Spacing.x, 0, 0));
            }
            else
            {
                m_HandleTf.localPosition = m_Child[m_ViewCnt + m_AddCnt - 1].localPosition - (new Vector3(0, m_Height + m_Spacing.y, 0));
            }
            m_Child.RemoveAt(0);
            m_Child.Add(m_HandleTf);

            m_FIdx = GetIdx(m_FIdx, true);
            m_LIdx = GetIdx(m_LIdx, true);
            Fill(m_HandleTf, m_LIdx);

            ResetSize();
        }
    }

    private void Fill(Transform tf, int idx)
    {
        if (tf == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (m_Debug)
        {
            Text tex = tf.FindChild("Text").GetComponent<Text>();
            tex.text = idx.ToString();
        }
#endif
        if (m_OnFillItem != null)
        {
            m_OnFillItem(tf, idx);
        }
    }

    private float m_BoundPos;
    private Vector2 m_OldDelta;
    private void ResetSize()
    {
        m_OldDelta = m_ContentRect.sizeDelta;
        if (m_Dir == Dir.Horizontal)
        {
            m_BoundPos = Mathf.Max(Mathf.Abs(m_Child[0].localPosition.x), Mathf.Abs(m_Child[m_ViewCnt + m_AddCnt - 1].localPosition.x));
            m_BoundPos = (m_BoundPos + m_Width * 0.5f) * 2;
            m_OldDelta.x = m_BoundPos;
            m_OldDelta.y = m_Height;
        }
        else
        {
            m_BoundPos = Mathf.Max(Mathf.Abs(m_Child[0].localPosition.y), Mathf.Abs(m_Child[m_ViewCnt + m_AddCnt - 1].localPosition.y));
            m_BoundPos = (m_BoundPos + m_Height * 0.5f) * 2;
            m_OldDelta.y = m_BoundPos;
            m_OldDelta.x = m_Width;
        }
        m_ContentRect.sizeDelta = m_OldDelta;
    }

    private float m_LastPos = 0f;
    private float m_NowPos;
    private float m_DoCenterDelta = 0f;
    private float m_DoCenterSpeed = 5f;
    private IEnumerator DoCenter()
    {
        if (m_Inited == false
            || m_Awaked == false
            || m_Center == false)
        {
            yield break;
        }

        m_LastPos = m_Content.localPosition.y;
        yield return new WaitForEndOfFrame();
        m_NowPos = m_Content.localPosition.y;

        while (Mathf.Abs(m_NowPos - m_LastPos) > 0.1f)
        {
            m_LastPos = m_NowPos;
            yield return new WaitForEndOfFrame();
            m_NowPos = m_Content.localPosition.y;
        }

        m_DoCenterDelta = 0f;
        if (m_Dir == Dir.Horizontal)
        {
            m_DoCenterDelta = m_Width;
        }
        else
        {
            m_DoCenterDelta = m_Height;
        }

        Vector3 ve;
        Transform centerTf = null;
        int centerIdx = 0;

        for (int i = 0; i < m_AddCnt + m_ViewCnt; i++)
        {
            ve = GetPos(m_Child[i]);
            if (m_Dir == Dir.Horizontal)
            {
                if (Mathf.Abs(ve.x) < Mathf.Abs(m_DoCenterDelta))
                {
                    centerTf = m_Child[i];
                    centerIdx = i;
                    m_DoCenterDelta = ve.x;
                }
            }
            else
            {
                if (Mathf.Abs(ve.y) < Mathf.Abs(m_DoCenterDelta))
                {
                    centerTf = m_Child[i];
                    centerIdx = i;
                    m_DoCenterDelta = ve.y;
                }
            }
        }

        if (centerTf != null
            && m_OnCenterItem != null)
        {
            centerIdx = GetIdx(m_FIdx + centerIdx + 1, false);
            m_OnCenterItem(centerTf, centerIdx);
        }

        ve = m_ContentRect.localPosition;
        if (m_Dir == Dir.Horizontal)
        {
            ve.x -= m_DoCenterDelta;
        }
        else
        {
            ve.y -= m_DoCenterDelta;
        }

        m_DoCenterSpeed = 5f + 5 * (Mathf.Abs(m_DoCenterDelta) / (0.4f * m_Height));
        iTween.MoveTo(m_Content.gameObject, iTween.Hash("position", ve,
            "isLocal", true, "speed", m_DoCenterSpeed, "easetype", iTween.EaseType.easeOutQuad));
    }
}