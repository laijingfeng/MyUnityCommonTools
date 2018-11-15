//Version: 2018-11-15-00

using Jerry;
using System;
using UnityEngine;

/// <summary>
/// 单例
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Reflection.Obfuscation(ApplyToMembers = true, Exclude = true, Feature = "renaming")]
public class Singleton<T>
{
    /// <summary>
    /// 单例
    /// </summary>
    private static T m_instance = default(T);

    /// <summary>
    /// 单例
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
/// <para>单例Mono</para>
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Reflection.Obfuscation(ApplyToMembers = true, Exclude = true, Feature = "renaming")]
public class SingletonMono<T> : MonoBehaviour where T : UnityEngine.Component
{
    /// <summary>
    /// 单例
    /// </summary>
    private static T m_instance = default(T);

    /// <summary>
    /// 单例
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
    /// <para>会给子类带来一个警告，注意用override</para>
    /// <para>这个不能去掉，预先挂载好的脚本走的是这个实例化</para>
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
            UnityEngine.Debug.LogError(string.Format("{0}的单例重复，新发现的忽略\n已有的路径:{1}\n新发现的路径:{2}",
                typeof(T).Name, oldPath, newPath));
        }
    }
}