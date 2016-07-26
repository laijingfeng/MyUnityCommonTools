using UnityEngine;
using JerryFsm;

public class Tr_Run_RunWay2FollowPlay : Transition
{
    public Tr_Run_RunWay2FollowPlay(int nID)
        : base(nID)
    {

    }

    public override bool Check()
    {
        if (Vector3.Distance((m_CurState.Mgr as MonsterStateMgr).Player.position,
            (m_CurState.Mgr as MonsterStateMgr).Self.position) < 2)
        {
            return true;
        }
        return base.Check();
    }
}
