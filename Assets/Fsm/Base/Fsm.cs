using System.Collections.Generic;
using UnityEngine;

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

            state.SetStateMgr(this);

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
    }
}