﻿using UnityEngine;
using Jerry;

public class PlayerAIMgr_Move : AIMgr
{
    public Transform[] path;

    public override void Start()
    {
        base.Start();

        StartFsm();
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (m_Fsm.Running)
            {
                StopFsm();
            }
            else
            {
                StartFsm();
            }
        }
    }

    public override void MakeFsm()
    {
        m_Fsm = new PlayerFsm(path);
        m_Fsm.m_DoDrawSelected = true;

        PlayerState_Walk walk = new PlayerState_Walk(PlayerStateID.Walk.GetHashCode());
        walk.AddTransition(new Tr_Move_Walk2Run(PlayerStateID.Run.GetHashCode()));
        m_Fsm.AddState(walk);

        PlayerState_Run run = new PlayerState_Run(PlayerStateID.Run.GetHashCode());
        run.AddTransition(new Tr_Move_Run2Walk(PlayerStateID.Walk.GetHashCode()));
        m_Fsm.AddState(run);
    }
}