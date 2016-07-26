using System.Collections.Generic;
using System;

namespace JerryFsm
{
    /// <summary>
    /// 转换条件
    /// </summary>
    public abstract class Transition
    {
        protected int m_NextID;
        public int NextID
        {
            get
            {
                return m_NextID;
            }
        }

        /// <summary>
        /// 判条件的时候要用到state的信息
        /// </summary>
        protected State m_CurState;

        public void SetState(State s)
        {
            m_CurState = s;
        }

        public Transition(int nID)
        {
            m_NextID = nID;
        }

        public virtual bool Check() { return false; }
    }

    public abstract class State
    {
        protected List<Transition> m_Transitions;
        protected int m_StateID;
        public int ID
        {
            get
            {
                return m_StateID;
            }
        }

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

        public State(int id)
        {
            m_StateID = id;
            m_Transitions = new List<Transition>();
        }

        public virtual void Enter() { }

        /// <summary>
        /// base.Update()需要执行
        /// </summary>
        public virtual void Update()
        {
            if (m_StateMgr == null)
            {
                return;
            }

            foreach (Transition tr in m_Transitions)
            {
                if (tr.Check())
                {
                    m_StateMgr.ChangeState(tr.NextID);
                    return;
                }
            }
        }

        public virtual void Exit() { }

        public void AddTransition<T>(int nextID) where T : Transition
        {
            T t = (T)Activator.CreateInstance(typeof(T), nextID);
            t.SetState(this);
            if (m_Transitions.Contains(t) == false)
            {
                m_Transitions.Add(t);
            }
        }
    }
}