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

        protected Fsm m_Fsm;

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
            if (m_Fsm == null)
            {
                return;
            }

            for (i = 0; i < m_TransitionCnt; i++)
            {
                if (m_Transitions[i] != null && m_Transitions[i].Check())
                {
                    m_Fsm.ChangeState(m_Transitions[i].NextID());
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
            Handles.Label(m_Fsm.Trans.position, Name());
#endif
        }
    }
}