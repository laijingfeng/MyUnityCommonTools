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
        MonsterFsm mgr = m_CurState.Mgr as MonsterFsm;
        if (Vector3.Distance(mgr.Player.position, mgr.Trans.position) < 2)
        {
            return true;
        }
        return false;
    }
}
