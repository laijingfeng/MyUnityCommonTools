using System.Collections.Generic;

namespace Jerry
{
    public class Fsm
    {
        protected List<State> m_States;

        private State m_CurState;

        private AIMgr m_AIMgr;

        /// <summary>
        /// 运行中
        /// </summary>
        private bool m_Running;

        public bool Running
        {
            get
            {
                return m_Running;
            }
        }

        public bool m_DoDraw;
        public bool m_DoDrawSelected;

        public State CurrentState
        {
            get
            {
                return m_CurState;
            }
        }

        public AIMgr GetMgr { get { return m_AIMgr; } }

        /// <summary>
        /// System Use
        /// </summary>
        /// <param name="aiMgr"></param>
        public void SetMgr(AIMgr aiMgr)
        {
            m_AIMgr = aiMgr;
        }

        public Fsm()
        {
            m_Running = false;
            m_DoDraw = false;
            m_DoDrawSelected = false;
            m_CurState = null;
            m_States = new List<State>();
        }

        public void Start()
        {
            if (m_CurState == null)
            {
                if (m_States.Count > 0)
                {
                    m_CurState = m_States[0];
                    m_CurState.Enter();
                }
            }
            m_Running = true;
        }

        public void Stop()
        {
            m_Running = false;
        }

        public void Update()
        {
            if (m_Running == false)
            {
                return;
            }

            if (m_CurState != null)
            {
                m_CurState.Update();
            }
        }

        public void AddState(State state)
        {
            if (state == null)
            {
                return;
            }

            state.SetFsm(this);

            if (m_States.Contains(state) == false)
            {
                m_States.Add(state);
            }
        }

        /// <summary>
        /// 可供外部调用，用来强制跳转
        /// </summary>
        /// <param name="stateID"></param>
        public void ChangeState(int stateID)
        {
            foreach (State state in m_States)
            {
                if (state.ID == stateID)
                {
                    m_CurState.Exit();
                    m_CurState = state;
                    m_CurState.Enter();
                    break;
                }
            }
        }

        public virtual void DrawSelected()
        {
            if (m_DoDrawSelected == false)
            {
                return;
            }
            if (m_CurState != null)
            {
                m_CurState.DrawSelected();
            }
        }

        public virtual void Draw()
        {
            if (m_DoDraw == false)
            {
                return;
            }

            if (m_CurState != null)
            {
                m_CurState.Draw();
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
            ret += string.Format("{0}\n\n", GetNode());
            foreach (State s in m_States)
            {
                ret += string.Format("{0}\n", s.GetNodes());
            }
            return ret;
        }

        public string GetSubGraph()
        {
            string ret = "";
            foreach (State s in m_States)
            {
                ret += string.Format("{0}\n", s.GetSubGraph());
            }
            return ret;
        }

        public string GetLinks()
        {
            string ret = "";
            //foreach (State s in m_States)
            //{
            //    ret += string.Format("{0}-->{1}\n", GetNodeName(), s.GetNodeName());
            //}
            foreach (State s in m_States)
            {
                ret += string.Format("{0}", s.GetLinks());
            }
            return ret;
        }

        #endregion Graph
    }
}