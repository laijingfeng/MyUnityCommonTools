using System;
using UnityEngine;

namespace Jerry
{
    /// <summary>
    /// 单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T>
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static T m_instance = default(T);

        /// <summary>
        /// 单例
        /// </summary>
        public static T Inst
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
	public class SingletonMono<T> : MonoBehaviour where T : UnityEngine.Component
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static T m_instance = default(T);
        
        /// <summary>
        /// <para>会给子类带来一个警告，注意用override</para>
        /// <para>这个不能去掉，预先挂载好的脚本走的是这个实例化</para>
        /// </summary>
        protected virtual void Awake()
        {
            if(m_instance == null)
            {
                m_instance = (T)(System.Object)(this);
            }
            else if(m_instance != (T)(System.Object)(this))
            {
                UnityEngine.Debug.LogError(string.Format("{0}的单例重复，新发现的忽略", typeof(T).Name));
            }
        }

        /// <summary>
        /// 单例
        /// </summary>
        public static T Inst
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
    }
}