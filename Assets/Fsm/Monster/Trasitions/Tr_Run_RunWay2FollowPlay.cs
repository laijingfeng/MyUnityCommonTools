﻿using UnityEngine;
using Jerry;

public class Tr_Run_RunWay2FollowPlay : Transition
{
    public Tr_Run_RunWay2FollowPlay(int nextID) : base(nextID) { }

    public override bool Check()
    {
        MonsterFsm fsm = m_CurState.CurFsm as MonsterFsm;
        if (Vector3.Distance(fsm.Player.position, fsm.GetMgr.transform.position) < 2)
        {
            return true;
        }
        return false;
    }
}