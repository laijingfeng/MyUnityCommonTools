using UnityEngine;
using JerryFsm;

public class PlayerAIMgr_Move : AIMgr
{
    public Transform[] path;

    public override void MakeFsm()
    {
        m_StateMgr = new PlayerStateMgr(this.transform, path);

        PlayerState_Walk walk = new PlayerState_Walk();
        walk.AddTransition(new Tr_Move_Walk2Run());
        m_StateMgr.AddState(walk);

        PlayerState_Run run = new PlayerState_Run();
        run.AddTransition(new Tr_Move_Run2Walk());
        m_StateMgr.AddState(run);
    }
}
