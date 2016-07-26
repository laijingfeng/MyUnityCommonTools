using UnityEngine;
using System.Collections;
using JerryFsm;

public class MonsterAIMgr_Idle : AIMgr
{
    public Transform[] path;
    public Transform player;

    public override void MakeFsm()
    {
        m_StateMgr = new MonsterStateMgr(player, this.transform);

        MonsterState_Idle idle = new MonsterState_Idle((int)MonsterStateID.Idle);
        idle.AddTransition<Tr_Idle_Idle2RunWay>((int)MonsterStateID.RunWay);
        m_StateMgr.AddState(idle);

        MonsterState_RunWay run = new MonsterState_RunWay((int)MonsterStateID.RunWay, path);
        run.AddTransition<Tr_Idle_RunWay2Idle>((int)MonsterStateID.Idle);
        m_StateMgr.AddState(run);
    }
}