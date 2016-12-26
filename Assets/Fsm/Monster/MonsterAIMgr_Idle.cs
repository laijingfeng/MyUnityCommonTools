using UnityEngine;
using Jerry;

public class MonsterAIMgr_Idle : AIMgr
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

        MonsterState_Idle idle = new MonsterState_Idle(MonsterStateID.Idle.GetHashCode());
        idle.AddTransition(new Tr_Idle_Idle2RunWay(MonsterStateID.RunWay.GetHashCode()));
        m_Fsm.AddState(idle);

        MonsterState_RunWay run = new MonsterState_RunWay(MonsterStateID.RunWay.GetHashCode(), path);
        run.AddTransition(new Tr_Idle_RunWay2Idle(MonsterStateID.Idle.GetHashCode()));
        m_Fsm.AddState(run);
    }
}