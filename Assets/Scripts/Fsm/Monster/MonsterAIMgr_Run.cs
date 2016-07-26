using UnityEngine;
using System.Collections;
using JerryFsm;

public class MonsterAIMgr_Run : AIMgr
{
    public Transform[] path;
    public Transform player;

    public override void MakeFsm()
    {
        m_StateMgr = new MonsterStateMgr(player, this.transform);

        MonsterState_RunWay run = new MonsterState_RunWay((int)MonsterStateID.RunWay, path);
        run.AddTransition<Tr_Run_RunWay2FollowPlay>((int)MonsterStateID.FollowPlayer);
        m_StateMgr.AddState(run);

        MonsterState_FollowPlayer follow = new MonsterState_FollowPlayer((int)MonsterStateID.FollowPlayer);
        follow.AddTransition<Tr_Run_FollowPlay2RunWay>((int)MonsterStateID.RunWay);
        m_StateMgr.AddState(follow);
    }
}
