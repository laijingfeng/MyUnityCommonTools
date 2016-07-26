using UnityEngine;
using JerryFsm;

public class PlayerAIMgr_Move : AIMgr
{
    public Transform[] path;
    
    public override void MakeFsm()
    {
        m_StateMgr = new PlayerStateMgr(this.transform, path);
        
        PlayerState_Walk walk = new PlayerState_Walk((int)PlayerStateID.Walk);
        walk.AddTransition<Tr_Move_Walk2Run>((int)PlayerStateID.Run);
        m_StateMgr.AddState(walk);

        PlayerState_Run run = new PlayerState_Run((int)PlayerStateID.Run);
        run.AddTransition<Tr_Move_Run2Walk>((int)PlayerStateID.Walk);
        m_StateMgr.AddState(run);
    }
}
