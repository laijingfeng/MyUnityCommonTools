using System.Collections.Generic;

namespace JerryFsm
{
    /// <summary>
    /// 转换条件
    /// </summary>
    public abstract class Transition
    {
        /// <summary>
        /// 判条件的时候要用到state的信息
        /// </summary>
        protected State m_CurState;

        public void SetState(State s)
        {
            m_CurState = s;
        }

        public abstract int NextID();
        public abstract bool Check();
    }
}