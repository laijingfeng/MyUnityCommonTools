using System.Collections.Generic;
using UnityEngine;

namespace JerryFsm
{
    public class Fsm
    {
        protected List<State> m_States;

        private State m_CurState;

        private Transform m_Trans;

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

        public State CurrentState
        {
            get
            {
                return m_CurState;
            }
        }

        public GameObject Go
        {
            get
            {
                return m_Trans.gameObject;
            }
        }

        public Transform Trans
        {
            get
            {
                return m_Trans;
            }
        }

        /// <summary>
        /// System Use
        /// </summary>
        /// <param name="tf"></param>
        public void SetTrans(Transform tf)
        {
            m_Trans = tf;
        }

        public Fsm()
        {
            m_Running = false;
            m_DoDraw = false;
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
                if (state.ID() == stateID)
                {
                    m_CurState.Exit();
                    m_CurState = state;
                    m_CurState.Enter();
                    break;
                }
            }
        }

        public virtual void Draw()
        {
#if UNITY_EDITOR
            if (m_DoDraw == false)
            {
                return;
            }

            if (m_CurState != null)
            {
                m_CurState.Draw();
            }
#endif
        }
    }
}
