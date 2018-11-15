//Version: 2018-11-15-00

using Jerry;
using System;
using UnityEngine;

/// <summary>
/// ����
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Reflection.Obfuscation(ApplyToMembers = true, Exclude = true, Feature = "renaming")]
public class Singleton<T>
{
    /// <summary>
    /// ����
    /// </summary>
    private static T m_instance = default(T);

    /// <summary>
    /// ����
    /// </summary>
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = (T)Activator.CreateInstance(typeof(T), true);
            }
            return m_instance;
        }
    }
}

/// <summary>
/// <para>����Mono</para>
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Reflection.Obfuscation(ApplyToMembers = true, Exclude = true, Feature = "renaming")]
public class SingletonMono<T> : MonoBehaviour where T : UnityEngine.Component
{
    /// <summary>
    /// ����
    /// </summary>
    private static T m_instance = default(T);

    /// <summary>
    /// ����
    /// </summary>
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<T>();
                if (m_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    m_instance = go.AddComponent<T>();
                }
            }
            return m_instance;
        }
    }

    /// <summary>
    /// <para>����������һ�����棬ע����override</para>
    /// <para>�������ȥ����Ԥ�ȹ��غõĽű��ߵ������ʵ����</para>
    /// </summary>
    protected virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = (T)(System.Object)(this);
        }
        else if(m_instance != (T)(System.Object)(this))
        {
            string oldPath = "";
            string newPath = "";
            if (m_instance.transform != null)
            {
                oldPath = JerryUtil.GetTransformHieraichyPath(m_instance.transform);
            }
            if (this.transform != null)
            {
                newPath = JerryUtil.GetTransformHieraichyPath(this.transform);
            }
            UnityEngine.Debug.LogError(string.Format("{0}�ĵ����ظ����·��ֵĺ���\n���е�·��:{1}\n�·��ֵ�·��:{2}",
                typeof(T).Name, oldPath, newPath));
        }
    }
}