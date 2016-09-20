using UnityEngine;
using JerryFsm;

public class MonsterAIMgr_Run : AIMgr
{
    public Transform[] path;
    public Transform player;

    public override void Start()
    {
        base.Start();

        StartFsm();
    }

    public override void MakeFsm()
    {
        m_Fsm = new MonsterFsm(player);
        m_Fsm.m_DoDraw = true;

        MonsterState_RunWay run = new MonsterState_RunWay(path);
        run.AddTransition(new Tr_Run_RunWay2FollowPlay());
        m_Fsm.AddState(run);

        MonsterState_FollowPlayer follow = new MonsterState_FollowPlayer();
        follow.AddTransition(new Tr_Run_FollowPlay2RunWay());
        m_Fsm.AddState(follow);
    }
}
