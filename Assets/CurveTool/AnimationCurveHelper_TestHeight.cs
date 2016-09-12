#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AnimationCurveHelper_TestHeight : MonoBehaviour
{
    public Transform m_Root;
    public bool m_Collect;

    void Start()
    {
        if (m_Root != null)
        {
            m_Anim = m_Root.GetComponent<Animator>();
        }

        Application.targetFrameRate = 30;
        m_Finish = false;
        m_Keys.Clear();
    }

    private Animator m_Anim;
    private bool m_Finish = false;
    private List<OneKey> m_Keys = new List<OneKey>();

    public class OneKey
    {
        public float key;
        public float val;
    }

    void Update()
    {
        Collect();
    }

    private void Collect()
    {
        if (m_Anim == null)
        {
            return;
        }

        AnimatorStateInfo info = m_Anim.GetCurrentAnimatorStateInfo(0);

        if (m_Collect == false)
        {
            Debug.LogWarning(string.Format("{0}f, {1}f", info.normalizedTime, this.transform.position.y));
            return;
        }

        if (m_Finish)
        {
            return;
        }

        if (info.normalizedTime > 1f)
        {
            m_Finish = true;
            OutKeys();
            return;
        }

        m_Keys.Add(new OneKey()
        {
            key = info.normalizedTime,
            val = this.transform.position.y,
        });
    }

    private void OutKeys()
    {
        if (m_Keys == null || m_Keys.Count < 1)
        {
            return;
        }

        float vv = 0.5f * (m_Keys[0].val + m_Keys[m_Keys.Count - 1].val);
        m_Keys.Insert(0, new OneKey()
        {
            key = 0,
            val = vv,
        });
        m_Keys.Add(new OneKey()
        {
            key = 1,
            val = vv,
        });
        float minVal = 100f;
        for (int i = 0, imax = m_Keys.Count; i < imax; i++)
        {
            if (m_Keys[i].val < minVal)
            {
                minVal = m_Keys[i].val;
            }
        }

        string str = string.Empty;

        for (int i = 0, imax = m_Keys.Count; i < imax; i++)
        {
            str += string.Format("{0},{1}", m_Keys[i].key, m_Keys[i].val - minVal);
            if (imax - 1 != i)
            {
                str += ";";
            }
        }

        Debug.Log(str);
    }
}
#endif