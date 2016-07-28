using UnityEngine;
using JerryFsm;

public class PlayerAIMgr_Move : AIMgr
{
    public Transform[] path;

    public override void MakeFsm()
    {
        m_Fsm = new PlayerFsm(path);
        m_Fsm.m_ShowStateName = true;
        
        PlayerState_Walk walk = new PlayerState_Walk();
        walk.AddTransition(new Tr_Move_Walk2Run());
        m_Fsm.AddState(walk);

        PlayerState_Run run = new PlayerState_Run();
        run.AddTransition(new Tr_Move_Run2Walk());
        m_Fsm.AddState(run);
    }
}
