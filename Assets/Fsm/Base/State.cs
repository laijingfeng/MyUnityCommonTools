using System.Collections.Generic;

namespace Jerry
{
    public abstract class State
    {
        private List<Transition> m_Transitions;
        private List<Action> m_Actions;
        private bool m_SequnceAction;

        private Fsm m_Fsm;
        public Fsm CurFsm
        {
            get
            {
                return m_Fsm;
            }
        }

        public void SetSequnceAction(bool sequnce)
        {
            m_SequnceAction = sequnce;
        }

        public void SetFsm(Fsm fsm)
        {
            m_Fsm = fsm;
        }

        public State(int id)
        {
            m_ID = id;
            m_Transitions = new List<Transition>();
            m_Actions = new List<Action>();
            m_SequnceAction = false;
        }

        private int m_ID;

        public int ID { get { return m_ID; } }

        /// <summary>
        /// base.Enter()需要执行
        /// </summary>
        public virtual void Enter()
        {
            foreach (Action ac in m_Actions)
            {
                if (ac.Started)
                {
                    ac.Reset();
                }
            }

            if (m_SequnceAction == false)
            {
                foreach (Action ac in m_Actions)
                {
                    if (ac.Started)
                    {
                        ac.Enter();
                    }
                }
            }
        }

        private bool m_HaveActionUpdate = false;

        /// <summary>
        /// base.Update()需要执行
        /// </summary>
        public virtual void Update()
        {
            if (m_Fsm == null)
            {
                return;
            }

            m_HaveActionUpdate = false;

            if (m_SequnceAction == false)
            {
                foreach (Action ac in m_Actions)
                {
                    if (ac.Finished == false)
                    {
                        ac.Update();
                        m_HaveActionUpdate = true;
                    }
                }
            }
            else
            {
                foreach (Action ac in m_Actions)
                {
                    if (ac.Finished == false)
                    {
                        if (ac.Started == false)
                        {
                            ac.Enter();
                        }
                        else
                        {
                            ac.Update();
                        }
                        m_HaveActionUpdate = true;
                        break;
                    }
                }
            }

            if (m_HaveActionUpdate)
            {
                return;
            }

            foreach (Transition tr in m_Transitions)
            {
                if (tr != null && tr.Check())
                {
                    m_Fsm.ChangeState(tr.NextID);
                    return;
                }
            }
        }

        public virtual void Exit()
        {
            foreach (Action ac in m_Actions)
            {
                if (ac.Started == true && ac.Finished == false)
                {
                    ac.Finish();
                }
            }
        }

        public void AddAction(Action a)
        {
            if (a == null)
            {
                return;
            }

            if (m_Actions.Contains(a) == false)
            {
                a.SetState(this);
                m_Actions.Add(a);
            }
        }

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