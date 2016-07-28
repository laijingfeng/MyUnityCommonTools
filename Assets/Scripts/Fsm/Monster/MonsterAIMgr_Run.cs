using UnityEngine;
using JerryFsm;

public class MonsterAIMgr_Run : AIMgr
{
    public Transform[] path;
    public Transform player;

    public override void MakeFsm()
    {
        m_StateMgr = new MonsterStateMgr(player, this.transform);

        MonsterState_RunWay run = new MonsterState_RunWay(path);
        run.AddTransition(new Tr_Run_RunWay2FollowPlay());
        m_StateMgr.AddState(run);

        MonsterState_FollowPlayer follow = new MonsterState_FollowPlayer();
        follow.AddTransition(new Tr_Run_FollowPlay2RunWay());
        m_StateMgr.AddState(follow);
    }
}
