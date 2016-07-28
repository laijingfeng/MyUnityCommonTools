using UnityEngine;
using JerryFsm;

public class Tr_Idle_Idle2RunWay : Transition
{
    public override int NextID()
    {
        return (int)MonsterStateID.RunWay;
    }

    public override bool Check()
    {
        MonsterStateMgr mgr = m_CurState.Mgr as MonsterStateMgr;
        if (Vector3.Distance(mgr.Player.position, mgr.Self.position) < 2)
        {
            return true;
        }
        return false;
    }
}
