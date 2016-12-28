
namespace Jerry
{
    /// <summary>
    /// 转换条件
    /// </summary>
    public abstract class Transition
    {
        /// <summary>
        /// 判条件的时候要用到state的信息
        /// </summary>
        private State m_CurState;

        protected State CurState { get { return m_CurState; } }

        public void SetState(State s)
        {
            m_CurState = s;
        }

        private int m_NextID;
        public int NextID { get { return m_NextID; } }

        public Transition(int nextID)
        {
            m_NextID = nextID;
        }

        public abstract bool Check();

        #region Graph

        public string GetNextNodeName()
        {
            return string.Format("{0}", NextID);
        }

        public string GetNodeName()
        {
            return string.Format("{0}", this.GetType());
        }

        #endregion Graph
    }
}