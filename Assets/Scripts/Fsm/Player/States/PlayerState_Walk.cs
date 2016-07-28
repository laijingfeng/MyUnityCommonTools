using UnityEngine;
using JerryFsm;

public class PlayerState_Walk : State
{
    public int frame;

    public override int ID()
    {
        return (int)PlayerStateID.Walk;
    }

    public override void Enter()
    {
        base.Enter();
        frame = 0;
    }

    public override void Update()
    {
        base.Update();

        frame++;

        PlayerStateMgr mgr = m_StateMgr as PlayerStateMgr;

        if (mgr.path == null || mgr.path.Length <= 0)
        {
            return;
        }

        Vector3 moveDir = mgr.path[mgr.curIdx].position - mgr.Self.position;
        if (moveDir.magnitude < 0.1f)
        {
            mgr.curIdx = (mgr.curIdx + 1) % mgr.path.Length;
            moveDir = mgr.path[mgr.curIdx].position - mgr.Self.position;
        }
        mgr.Self.rotation = Quaternion.LookRotation(moveDir);
        mgr.Self.position = mgr.Self.position + mgr.Self.forward * 0.015f;
    }
}
