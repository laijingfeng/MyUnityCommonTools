﻿using UnityEngine;

namespace JerryFsm
{
    public abstract class AIMgr : MonoBehaviour
    {
        protected Fsm m_Fsm;

        public virtual void Start()
        {
            MakeFsm();
            if (m_Fsm != null)
            {
                m_Fsm.SetTrans(this.transform);
            }
        }

        public void StartFsm()
        {
            if (m_Fsm != null)
            {
                m_Fsm.Start();
            }
        }

        public void StopFsm()
        {
            if (m_Fsm != null)
            {
                m_Fsm.Stop();
            }
        }

        public abstract void MakeFsm();
        
        public virtual void Update()
        {
            if (m_Fsm != null)
            {
                m_Fsm.Update();
            }
        }

        public virtual void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (m_Fsm != null)
            {
                m_Fsm.DrawSelected();
            }
#endif
        }

        public virtual void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (m_Fsm != null)
            {
                m_Fsm.Draw();
            }
#endif
        }
    }
}