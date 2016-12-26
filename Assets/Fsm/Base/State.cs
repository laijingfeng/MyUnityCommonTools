using System.Collections.Generic;
using UnityEngine;

namespace Jerry
{
    public abstract class State
    {
        private int i, m_TransitionCnt;

        private List<Transition> m_Transitions;

        private Fsm m_Fsm;

        public Fsm CurFsm
        {
            get
            {
                return m_Fsm;
            }
        }

        public void SetStateMgr(Fsm mgr)
        {
            m_Fsm = mgr;
        }

        public State(int id)
        {
            m_ID = id;
            m_Transitions = new List<Transition>();
        }

        private int m_ID;

        public int ID { get { return m_ID; } }

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
            if (m_Fsm == null)
            {
                return;
            }
            
            for (i = 0; i < m_TransitionCnt; i++)
            {
                if (m_Transitions[i] != null && m_Transitions[i].Check())
                {
                    m_Fsm.ChangeState(m_Transitions[i].NextID);
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

            if (m_Transitions.Contains(t) == false)
            {
                t.SetState(this);
                m_Transitions.Add(t);
            }
        }

        public virtual void Draw()
        {
        }

        public virtual void DrawSelected()
        {
        }
    }
}