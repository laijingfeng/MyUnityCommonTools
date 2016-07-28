using System.Collections.Generic;

namespace JerryFsm
{
    /// <summary>
    /// 转换条件
    /// </summary>
    public abstract class Transition
    {
        /// <summary>
        /// 判条件的时候要用到state的信息
        /// </summary>
        protected State m_CurState;

        public void SetState(State s)
        {
            m_CurState = s;
        }

        public abstract int NextID();
        public abstract bool Check();
    }

    public abstract class State
    {
        private int i, m_TransitionCnt;

        protected List<Transition> m_Transitions;

        protected StateMgr m_StateMgr;

        public StateMgr Mgr
        {
            get
            {
                return m_StateMgr;
            }
        }

        public void SetStateMgr(StateMgr mgr)
        {
            m_StateMgr = mgr;
        }

        public State()
        {
            m_Transitions = new List<Transition>();
        }

        /// <summary>
        /// ID
        /// </summary>
        /// <returns></returns>
        public abstract int ID();

        /// <summary>
        /// base.Enter()需要执行
        /// </summary>
        public virtual void Enter()
        {
            m_TransitionCnt = m_Transitions.Count;
        }

        /// <summary>
        /// base.Update()需要执行
        /// </summary>
        public virtual void Update()
        {
            if (m_StateMgr == null)
            {
                return;
            }

            for (i = 0; i < m_TransitionCnt; i++)
            {
                if (m_Transitions[i] != null && m_Transitions[i].Check())
                {
                    m_StateMgr.ChangeState(m_Transitions[i].NextID());
                    return;
                }
            }
        }

        public virtual void Exit() { }

        public void AddTransition(Transition t)
        {
            if (t == null)
            {
                return;
            }

            t.SetState(this);
            if (m_Transitions.Contains(t) == false)
            {
                m_Transitions.Add(t);
            }
        }
    }
}