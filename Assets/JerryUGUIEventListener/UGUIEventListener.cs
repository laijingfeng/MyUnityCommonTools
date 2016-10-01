using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace Jerry
{
    public class UGUIEventListener : EventTrigger
    {
        private object[] m_UserData;
        private bool m_CanSelected;

        #region 事件

        /// <summary>
        /// 点击1
        /// </summary>
        public Action<GameObject> onClick;
        /// <summary>
        /// 点击2，和点击1相比，回调参数不一样
        /// </summary>
        public Action<GameObject, BaseEventData> onClick2;

        public Action<GameObject> onUp;
        public Action<GameObject, BaseEventData> onUp2;

        public Action<GameObject> onDown;
        public Action<GameObject, BaseEventData> onDown2;

        #endregion 事件

        #region 对外接口

        /// <summary>
        /// 获得监听
        /// </summary>
        /// <param name="go"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static UGUIEventListener Get(GameObject go, object[] userData = null, bool canSelected = true)
        {
            if (go == null)
            {
                return null;
            }
            UGUIEventListener listener = go.GetComponent<UGUIEventListener>();
            if (listener == null)
            {
                listener = go.AddComponent<UGUIEventListener>();
            }
            listener.m_UserData = userData;
            listener.m_CanSelected = canSelected;
            return listener;
        }

        /// <summary>
        /// 获得用户数据
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static object[] GetData(GameObject go)
        {
            if (go == null)
            {
                return null;
            }
            UGUIEventListener listener = go.GetComponent<UGUIEventListener>();
            if (listener != null)
            {
                return listener.m_UserData;
            }
            return null;
        }

        /// <summary>
        /// 设置用户数据
        /// </summary>
        /// <param name="userData"></param>
        public void SetData(object[] userData)
        {
            this.m_UserData = userData;
        }

        /// <summary>
        /// 设置能被选中
        /// </summary>
        /// <param name="canSelected"></param>
        public void SetCanSelected(bool canSelected)
        {
            this.m_CanSelected = canSelected;
        }

        #endregion 对外接口

        #region 事件处理

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (this.onClick != null)
            {
                this.onClick(this.gameObject);
            }
            if (this.onClick2 != null)
            {
                this.onClick2(this.gameObject, eventData);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null)
            {
                onUp(this.gameObject);
            }
            if (onUp2 != null)
            {
                onUp2(this.gameObject, eventData);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (this.m_CanSelected)
            {
                EventSystem.current.SetSelectedGameObject(this.gameObject);
            }
            else
            {
                //Button组件会自动设置为true，不要的话，主动设置为null
                EventSystem.current.SetSelectedGameObject(null);
            }

            if (this.onDown != null)
            {
                this.onDown(this.gameObject);
            }
            if (this.onDown2 != null)
            {
                this.onDown2(this.gameObject, eventData);
            }
        }

        #endregion 事件处理
    }
}