using UnityEngine;

namespace JerryFsm
{
    public abstract class AIMgr : MonoBehaviour
    {
        protected Fsm m_Fsm;

        void Start()
        {
            MakeFsm();
            if (m_Fsm != null)
            {
                m_Fsm.SetTrans(this.transform);
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