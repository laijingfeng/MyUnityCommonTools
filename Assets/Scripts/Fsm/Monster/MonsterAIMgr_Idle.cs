using UnityEngine;
using JerryFsm;

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

        MonsterState_Idle idle = new MonsterState_Idle();
        idle.AddTransition(new Tr_Idle_Idle2RunWay());
        m_Fsm.AddState(idle);

        MonsterState_RunWay run = new MonsterState_RunWay(path);
        run.AddTransition(new Tr_Idle_RunWay2Idle());
        m_Fsm.AddState(run);
    }
}