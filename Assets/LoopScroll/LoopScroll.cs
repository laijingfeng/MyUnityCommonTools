using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Jerry;

public class LoopScroll : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    /// <summary>
    /// 方向
    /// </summary>
    public enum Dir
    {
        Horizontal,
        Vertical,
    }

    /// <summary>
    /// 填充和居中定位回调
    /// </summary>
    /// <param name="item"></param>
    /// <param name="realIndex"></param>
    public delegate void OnFillItem(Transform item, int realIndex);

    #region 对外变量

    /// <summary>
    /// 填充回调
    /// </summary>
    public OnFillItem m_OnFillItem;

    /// <summary>
    /// 居中定位回调，居中好了
    /// </summary>
    public OnFillItem m_OnCenterItem;

    /// <summary>
    /// 预设
    /// </summary>
    public GameObject m_Prefab;

    /// <summary>
    /// 对元素扩展一些大小，做间隔
    /// </summary>
    public Vector2 m_OutSide = new Vector2(2, 2);

    /// <summary>
    /// 第一个显示的idx，[0,m_TotalCnt)
    /// </summary>
    public int m_StartIdx = 0;

    /// <summary>
    /// 方向
    /// </summary>
    public Dir m_Dir = Dir.Horizontal;

    /// <summary>
    /// 是否居中定位
    /// </summary>
    public bool m_Center = false;

    /// <summary>
    /// 是否循环
    /// </summary>
    public bool m_Loop = false;

    /// <summary>
    /// 总数量
    /// </summary>
    public int m_TotalCnt = 100;

    /// <summary>
    /// 每几帧刷新一次
    /// </summary>
    public int m_RefreshRate = 2;

    public bool m_Debug = false;

    /// <summary>
    /// 两端格增加的数量，缓冲数量
    /// </summary>
    public int m_AddCnt = 1;

    #endregion 对外变量

    /// <summary>
    /// 元素的宽度
    /// </summary>
    private float m_PrefabWidth = 100;

    /// <summary>
    /// 元素的高度
    /// </summary>
    private float m_PrefabHeight = 100;

    /// <summary>
    /// 实例化元素(数量是m_ListTotal)列表里负方向可视范围外的第一个元素索引
    /// </summary>
    private int m_JudgeF;
    
    /// <summary>
    /// 实例化元素(数量是m_ListTotal)列表里正方向可视范围外的第一个元素索引
    /// </summary>
    private int m_JudgeL;
    
    /// <summary>
    /// 实例化元素的数量
    /// </summary>
    private int m_ListTotal;

    /// <summary>
    /// 填充元素后，边缘位置的绝对值
    /// </summary>
    private float m_RefreshJudgePos;

    /// <summary>
    /// 创建对象第一个在数据列表里的索引
    /// </summary>
    private int m_FIdx = 0;
    
    /// <summary>
    /// 创建对象最后一个数据列表里的索引
    /// </summary>
    private int m_LIdx = 0;

    /// <summary>
    /// m_JudgeF的位置
    /// </summary>
    private Vector3 m_PosJudgeF;

    /// <summary>
    /// m_JudgeL的位置
    /// </summary>
    private Vector3 m_PosJudgeL;

    private ScrollRect m_ScrollRect;
    private RectTransform m_ScrollRectTrans;

    private RectTransform m_ContentTrans;
    private RectTransform m_PrefabTrans;

    /// <summary>
    /// 一屏能看见的数量
    /// </summary>
    private int m_ViewCnt = 3;

    private Transform m_HandleTf;

    private List<Transform> m_Child = new List<Transform>();

    /// <summary>
    /// 启动了
    /// </summary>
    private bool m_Awaked = false;
    private bool m_Inited = false;
    /// <summary>
    /// 设置好了
    /// </summary>
    private bool m_HadSet = false;

    void Awake()
    {
        m_ScrollRect = this.transform.GetComponent<ScrollRect>();
        if (m_ScrollRect == null)
        {
            Debug.LogError("Need ScrollRect");
            return;
        }

        m_ContentTrans = m_ScrollRect.content;
        if (m_ContentTrans == null)
        {
            Debug.LogError("ScrollRect.Content is null");
            return;
        }

        m_ContentTrans.pivot = Vector2.one * 0.5f;
        m_ContentTrans.anchorMin = Vector2.one * 0.5f;
        m_ContentTrans.anchorMax = Vector2.one * 0.5f;

        m_ScrollRectTrans = m_ScrollRect.GetComponent<RectTransform>();
        m_ScrollRectTrans.pivot = Vector2.one * 0.5f;
        m_ScrollRectTrans.anchorMin = Vector2.one * 0.5f;
        m_ScrollRectTrans.anchorMax = Vector2.one * 0.5f;

        m_Awaked = true;
        TryDoInit();
    }

    void Start()
    {
#if UNITY_EDITOR
        if (m_Debug)
        {
            ApplySetting();
        }
#endif
    }

    #region 对外接口

    public void Reset()
    {
        m_Inited = false;
        m_HadSet = false;
        m_ScrollRect.enabled = false;
    }

    /// <summary>
    /// 应用设置
    /// </summary>
    public void ApplySetting()
    {
        if (m_AddCnt <= 0)
        {
            Debug.LogError("AddCnt at least one");
            return;
        }

        m_PrefabTrans = m_Prefab.GetComponent<RectTransform>();
        m_PrefabTrans.pivot = Vector2.one * 0.5f;
        m_PrefabTrans.anchorMin = Vector2.one * 0.5f;
        m_PrefabTrans.anchorMax = Vector2.one * 0.5f;

        m_PrefabWidth = m_PrefabTrans.sizeDelta.x + m_OutSide.x;
        m_PrefabHeight = m_PrefabTrans.sizeDelta.y + m_OutSide.y;

        Debug.LogError("size " + m_PrefabTrans.sizeDelta);

        float viewCnt = 0;
        switch (m_Dir)
        {
            case Dir.Horizontal:
                {
                    viewCnt = m_ScrollRectTrans.sizeDelta.x / m_PrefabWidth;
                }
                break;
            case Dir.Vertical:
                {
                    viewCnt = m_ScrollRectTrans.sizeDelta.y / m_PrefabHeight;
                }
                break;
        }
        m_ViewCnt = (int)viewCnt;
        if (viewCnt > m_ViewCnt * 1.0f)
        {
            m_ViewCnt++;
        }

        m_Frame = 0;

        m_ListTotal = m_ViewCnt + 2 * m_AddCnt;
        
        switch (m_Dir)
        {
            case Dir.Horizontal:
                {
                    m_RefreshJudgePos = 0.5f * (m_ViewCnt * m_PrefabWidth);
                }
                break;
            case Dir.Vertical:
                {
                    m_RefreshJudgePos = 0.5f * (m_ViewCnt * m_PrefabHeight);
                }
                break;
        }

        m_HadSet = true;

        TryDoInit();
    }

    /// <summary>
    /// 关闭的时候清理垃圾
    /// </summary>
    public void Clean()
    {
        m_HadSet = false;
        m_Inited = false;
        DoClean();
    }

    #endregion 对外接口

    /// <summary>
    /// 清理
    /// </summary>
    private void DoClean()
    {
        if (m_Center)
        {
            iTween.Stop(m_ContentTrans.gameObject);
            this.StopCoroutine("DoCenter");
        }

        m_ScrollRect.onValueChanged.RemoveListener(OnScrollChange);
        JerryUtil.DestroyAllChildren(m_ContentTrans);
        m_Child.Clear();
        m_ScrollRect.enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_Center)
        {
            iTween.Stop(m_ContentTrans.gameObject);
            this.StopCoroutine("DoCenter");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_Center)
        {
            iTween.Stop(m_ContentTrans.gameObject);
            this.StopCoroutine("DoCenter");
            this.StartCoroutine("DoCenter");
        }
    }

    private void TryDoInit()
    {
        if (m_Awaked == false
            || m_HadSet == false)
        {
            return;
        }

        this.StartCoroutine(IE_DoInit());
    }

    private IEnumerator IE_DoInit()
    {
        DoClean();

        yield return new WaitForEndOfFrame();

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

        if (m_Center)
        {
            m_Loop = true;
        }

        m_LastPos = 0f;
        //m_ListTotal = 8
        //0(m_JudgeF) [1 2 3 4 5] 6(m_JudgeL)
        m_JudgeF = m_AddCnt - 1;
        m_JudgeL = m_ListTotal - m_AddCnt;

        Vector3 firstPos = Vector3.zero;//填充元素列表里第一个的位置
        switch (m_Dir)
        {
            case Dir.Horizontal:
                {
                    firstPos.x = ((int)(m_ListTotal / 2)) * m_PrefabWidth;
                    if ((m_ListTotal & 1) == 0)
                    {
                        firstPos.x += 0.5f * m_PrefabWidth;
                    }
                }
                break;
            case Dir.Vertical:
                {
                    firstPos.y = ((int)(m_ListTotal / 2)) * m_PrefabHeight;
                    if ((m_ListTotal & 1) == 0)
                    {
                        firstPos.y -= 0.5f * m_PrefabHeight;
                    }
                }
                break;
        }

        Debug.LogError("y0=" + firstPos.y + " " + m_PrefabHeight + " " + m_ListTotal);

        m_ContentTrans.sizeDelta = new Vector2(10, 10);

        if (m_Center)
        {
            m_ContentTrans.anchoredPosition = Vector2.zero;
            m_FIdx = GetIdx(m_StartIdx + 1 - m_ListTotal / 2, false);
        }
        else
        {
            m_ContentTrans.anchoredPosition = Vector2.zero;
            m_FIdx = GetIdx(m_StartIdx - m_AddCnt + 1, false);
        }

        Debug.LogError("FIdx=" + m_FIdx);

        for (int i = 0; i < m_ListTotal; i++)
        {
            GameObject go = CloneGo();

            go.transform.localPosition = firstPos;
            m_Child.Add(go.transform);
            Fill(go.transform, GetIdx(m_FIdx + i + 1, false));

            switch (m_Dir)
            {
                case Dir.Horizontal:
                    {
                        firstPos.x += m_PrefabWidth;
                    }
                    break;
                case Dir.Vertical:
                    {
                        firstPos.y -= m_PrefabHeight;
                    }
                    break;
            }
        }

        m_LIdx = GetIdx(m_FIdx + m_ListTotal, false);

        Debug.LogError("LIdx=" + m_LIdx);

        yield return new WaitForEndOfFrame();

        ResetSize();

        yield return new WaitForEndOfFrame();

        m_ScrollRect.enabled = true;
        m_Inited = true;

        m_ScrollRect.onValueChanged.AddListener(OnScrollChange);
    }

    private GameObject CloneGo()
    {
        GameObject go = GameObject.Instantiate(m_Prefab);
        go.transform.SetParent(m_ContentTrans);
        go.SetActive(true);
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);
        return go;
    }

    /// <summary>
    /// 获得idx
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="next">上一个或下一个</param>
    /// <returns></returns>
    private int GetIdx(int idx, bool next)
    {
        int ret;
        if (next)
        {
            ret = idx + 1;
            if (m_Loop)
            {
                ret = (ret + m_TotalCnt) % m_TotalCnt;
            }
            else
            {
                if (ret >= m_TotalCnt)
                {
                    ret = m_TotalCnt - 1;
                }
            }
        }
        else
        {
            ret = idx - 1;
            if (m_Loop)
            {
                ret = (ret + m_TotalCnt) % m_TotalCnt;
            }
            else
            {
                if (ret < 0)
                {
                    ret = 0;
                }
            }
        }
        return ret;
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
                return (m_PosJudgeF.x > -m_RefreshJudgePos);
            }
            else
            {
                if (m_Loop == false && m_LIdx == m_TotalCnt - 1)
                {
                    return false;
                }
                return (m_PosJudgeL.x < m_RefreshJudgePos);
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
                return (m_PosJudgeF.y < m_RefreshJudgePos);
            }
            else
            {
                if (m_Loop == false && m_LIdx == m_TotalCnt - 1)
                {
                    return false;
                }
                return (m_PosJudgeL.y > -m_RefreshJudgePos);
            }
        }
    }

    /// <summary>
    /// 减小刷新频率，当前帧
    /// </summary>
    private int m_Frame = 0;

    private void OnScrollChange(Vector2 vet)
    {
        if (m_Awaked == false
            || m_Inited == false)
        {
            return;
        }

        m_Frame = (m_Frame + 1) % m_RefreshRate;
        if (m_Frame % m_RefreshRate != 0)
        {
            return;
        }

        m_PosJudgeF = GetPos(m_Child[m_JudgeF]);
        m_PosJudgeL = GetPos(m_Child[m_JudgeL]);

        if (NeedRefresh(true))
        {
            m_HandleTf = m_Child[m_ListTotal - 1];

            if (m_Dir == Dir.Horizontal)
            {
                m_HandleTf.localPosition = m_Child[0].localPosition - (new Vector3(m_PrefabWidth, 0, 0));
            }
            else
            {
                m_HandleTf.localPosition = m_Child[0].localPosition + (new Vector3(0, m_PrefabHeight, 0));
            }
            m_Child.RemoveAt(m_ListTotal - 1);
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
                m_HandleTf.localPosition = m_Child[m_ListTotal - 1].localPosition + (new Vector3(m_PrefabWidth, 0, 0));
            }
            else
            {
                m_HandleTf.localPosition = m_Child[m_ListTotal - 1].localPosition - (new Vector3(0, m_PrefabHeight, 0));
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
        m_OldDelta = m_ContentTrans.sizeDelta;
        if (m_Dir == Dir.Horizontal)
        {
            m_BoundPos = Mathf.Max(Mathf.Abs(m_Child[0].localPosition.x), Mathf.Abs(m_Child[m_ListTotal - 1].localPosition.x));
            m_BoundPos = 2 * m_BoundPos + m_PrefabWidth;
            m_OldDelta.x = m_BoundPos;
            m_OldDelta.y = m_PrefabHeight;
        }
        else
        {
            m_BoundPos = Mathf.Max(Mathf.Abs(m_Child[0].localPosition.y), Mathf.Abs(m_Child[m_ListTotal - 1].localPosition.y));
            m_BoundPos = 2 * m_BoundPos + m_PrefabHeight;
            m_OldDelta.y = m_BoundPos;
            m_OldDelta.x = m_PrefabWidth;
        }
        m_ContentTrans.sizeDelta = m_OldDelta;
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

        if (m_Dir == Dir.Horizontal)
        {
            m_LastPos = m_ContentTrans.localPosition.x;
            yield return new WaitForEndOfFrame();
            m_NowPos = m_ContentTrans.localPosition.x;

            while (Mathf.Abs(m_NowPos - m_LastPos) > 0.1f)
            {
                m_LastPos = m_NowPos;
                yield return new WaitForEndOfFrame();
                m_NowPos = m_ContentTrans.localPosition.x;
            }
        }
        else
        {
            m_LastPos = m_ContentTrans.localPosition.y;
            yield return new WaitForEndOfFrame();
            m_NowPos = m_ContentTrans.localPosition.y;

            while (Mathf.Abs(m_NowPos - m_LastPos) > 0.1f)
            {
                m_LastPos = m_NowPos;
                yield return new WaitForEndOfFrame();
                m_NowPos = m_ContentTrans.localPosition.y;
            }
        }

        m_DoCenterDelta = 0f;
        if (m_Dir == Dir.Horizontal)
        {
            m_DoCenterDelta = m_PrefabWidth;
        }
        else
        {
            m_DoCenterDelta = m_PrefabHeight;
        }

        Vector3 ve;
        Transform centerTf = null;
        int centerIdx = 0;

        for (int i = 0; i < m_ListTotal; i++)
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

        ve = m_ContentTrans.localPosition;
        if (m_Dir == Dir.Horizontal)
        {
            ve.x -= m_DoCenterDelta;
        }
        else
        {
            ve.y -= m_DoCenterDelta;
        }

        m_DoCenterSpeed = 5f + 5 * (Mathf.Abs(m_DoCenterDelta) / (0.4f * m_PrefabHeight));
        iTween.MoveTo(m_ContentTrans.gameObject, iTween.Hash("position", ve,
            "isLocal", true, "speed", m_DoCenterSpeed, "easetype", iTween.EaseType.easeOutQuad));
    }
}