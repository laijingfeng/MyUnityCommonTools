using System.Collections.Generic;

namespace JerryFsm
{
    public class StateMgr
    {
        protected List<State> m_States;

        private State m_CurState;

        public State CurrentState
        {
            get
            {
                return m_CurState;
            }
        }

        public StateMgr()
        {
            m_CurState = null;
            m_States = new List<State>();
        }

        public void Update()
        {
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

            state.SetStateMgr(this);

            if (m_States.Count == 0)
            {
                m_States.Add(state);
                m_CurState = state;
                return;
            }

            if (m_States.Contains(state) == false)
            {
                m_States.Add(state);
            }
        }

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
    }
}
