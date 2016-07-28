using UnityEngine;

namespace JerryFsm
{
    public abstract class AIMgr : MonoBehaviour
    {
        protected StateMgr m_StateMgr;

        void Start()
        {
            MakeFsm();
        }

        public abstract void MakeFsm();
        
        public virtual void Update()
        {
            if (m_StateMgr != null)
            {
                m_StateMgr.Update();
            }
        }
    }
}