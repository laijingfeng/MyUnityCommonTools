using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JerryFsm
{
    public class Fsm
    {
        protected List<State> m_States;

        private State m_CurState;

        private Transform m_Trans;

        public bool m_ShowStateName;

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
            m_ShowStateName = false;
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
            if (m_ShowStateName)
            {
                if (m_CurState != null)
                {
                    Handles.Label(m_Trans.position, m_CurState.Name());
                }
            }
        }
#endif
    }
}
