using UnityEngine;
using System.Collections;

namespace JerryFsm
{
    public class AIMgr : MonoBehaviour
    {
        protected StateMgr m_StateMgr;

        void Start()
        {
            MakeFsm();
        }

        public virtual void MakeFsm()
        {
            m_StateMgr = null;
        }

        public virtual void Update()
        {
            if (m_StateMgr != null)
            {
                m_StateMgr.Update();
            }
        }
    }
}