using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JerryFsm
{
    public abstract class State
    {
        private int i, m_TransitionCnt;

        protected List<Transition> m_Transitions;

        protected Fsm m_StateMgr;

        public Fsm Mgr
        {
            get
            {
                return m_StateMgr;
            }
        }

        public void SetStateMgr(Fsm mgr)
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

        public virtual void Draw()
        {
        }
    }

    public class DrawNameState : State
    {
        public virtual string Name()
        {
            return ID().ToString();
        }

        public override int ID()
        {
            return 0;
        }

        public override void Draw()
        {
            base.Draw();

#if UNITY_EDITOR
            Handles.Label(m_StateMgr.Trans.position, Name());
#endif
        }
    }
}