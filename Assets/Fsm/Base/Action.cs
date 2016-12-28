
namespace Jerry
{
    public class Action
    {
        public State m_State;
        public State CurState { get { return m_State; } }

        private bool m_Finished;
        private bool m_Started;

        public bool Finished { get { return m_Finished; } }
        public bool Started { get { return m_Started; } }

        public void SetState(State state)
        {
            m_State = state;
            Reset();
        }

        public virtual void Reset()
        {
            m_Finished = false;
            m_Started = false;
        }

        public virtual void Enter()
        {
            m_Started = true;
        }

        public virtual void Update()
        {
            if (m_State == null
                || m_Started == false
                || m_Finished == true)
            {
                return;
            }
        }

        public virtual void Exit()
        {
        }

        public virtual void Finish()
        {
            m_Finished = true;
            Exit();
        }

        #region Graph

        public string GetNode()
        {
            return string.Format("{0}>{1}]", GetNodeName(), this.GetType());
        }

        public string GetNodeName()
        {
            return string.Format("{0}.{1}", CurState.GetNodeName(), this.GetType());
        }

        #endregion Graph
    }
}