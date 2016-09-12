#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AnimationCurveHelper_Test : MonoBehaviour
{
    private Animator anim;
    private AudioSource m_AS;
    public AudioClip m_TestAudio;
    public float m_Interval = 0.5f;
    public float m_Low = 0.001f;

    public class CheckData
    {
        public string name;
        public float lastTime;
    }

    private CheckData m_RightHand;
    private CheckData m_LeftHand;
    private CheckData m_RightFoot;
    private CheckData m_LeftFoot;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        m_RightHand = new CheckData()
        {
            lastTime = -10f,
            name = "RightHand",
        };
        m_LeftHand = new CheckData()
        {
            lastTime = -10f,
            name = "LeftHand",
        };
        m_RightFoot = new CheckData()
        {
            lastTime = -10f,
            name = "RightFoot",
        };
        m_LeftFoot = new CheckData()
        {
            lastTime = -10f,
            name = "LeftFoot",
        };
    }

    private AudioSource GetAS()
    {
        if (m_AS == null)
        {
            m_AS = this.transform.GetComponent<AudioSource>();
            if (m_AS == null)
            {
                m_AS = this.gameObject.AddComponent<AudioSource>();
            }
        }
        return m_AS;
    }

    void Update()
    {
        if (anim == null)
        {
            return;
        }

        DoCheck(m_RightFoot);
        DoCheck(m_LeftFoot);
        DoCheck(m_RightHand);
        DoCheck(m_LeftHand);
    }

    private void DoCheck(CheckData data)
    {
        if (data == null)
        {
            return;
        }

        if (anim.GetFloat(data.name) <= m_Low
            && Time.realtimeSinceStartup - data.lastTime > m_Interval)
        {
            data.lastTime = Time.realtimeSinceStartup;
            Debug.LogError("hit " + data.name);
            PlaySound();
        }
    }

    private void PlaySound()
    {
        if (m_TestAudio == null)
        {
            return;
        }
        GetAS().PlayOneShot(m_TestAudio);
    }
}
#endif