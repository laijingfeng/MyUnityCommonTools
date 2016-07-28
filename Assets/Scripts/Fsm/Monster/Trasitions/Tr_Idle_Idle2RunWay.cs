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
        MonsterFsm fsm = m_CurState.CurFsm as MonsterFsm;
        if (Vector3.Distance(fsm.Player.position, fsm.Trans.position) < 2)
        {
            return true;
        }
        return false;
    }
}
