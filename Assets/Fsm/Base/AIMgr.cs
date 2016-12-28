using UnityEngine;

//version: 2016-12-28-02
namespace Jerry
{
    public abstract class AIMgr : MonoBehaviour
    {
        protected Fsm m_Fsm;

        public virtual void Start()
        {
            MakeFsm();
            if (m_Fsm != null)
            {
                m_Fsm.SetMgr(this);
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
            if (m_Fsm != null)
            {
                m_Fsm.DrawSelected();
            }
        }

        public virtual void OnDrawGizmos()
        {
            if (m_Fsm != null)
            {
                m_Fsm.Draw();
            }
        }

        #region Graph

        public string GetNode()
        {
            return string.Format("{0}[{1}]", GetNodeName(), this.GetType());
        }

        public string GetNodeName()
        {
            return string.Format("{0}", this.GetType());
        }

        public string GetNodes()
        {
            string ret = "";
            ret += string.Format("{0}\n", GetNode());
            ret += string.Format("{0}", m_Fsm.GetNodes());
            return ret;
        }

        public string GetSubGraph()
        {
            return m_Fsm.GetSubGraph();
        }

        public string GetLinks()
        {
            string ret = "";
            ret += string.Format("{0}-->{1}\n", GetNodeName(), m_Fsm.GetNodeName());
            ret += m_Fsm.GetLinks();
            return ret;
        }

        public string GetGraph()
        {
            string ret = "";
            ret += string.Format("graph TB\n");
            ret += GetNodes();
            ret += GetSubGraph();
            ret += GetLinks();
            return ret;
        }

        #endregion Graph
    }
}