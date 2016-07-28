using UnityEngine;
using JerryFsm;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

    public override void Draw()
    {
        base.Draw();
#if UNITY_EDITOR
        Handles.Label(m_StateMgr.Trans.position, "Walk");
#endif
    }

    public override void Update()
    {
        base.Update();
        
        frame++;

        PlayerFsm mgr = m_StateMgr as PlayerFsm;

        if (mgr.path == null || mgr.path.Length <= 0)
        {
            return;
        }

        Vector3 moveDir = mgr.path[mgr.curIdx].position - mgr.Trans.position;
        if (moveDir.magnitude < 0.1f)
        {
            mgr.curIdx = (mgr.curIdx + 1) % mgr.path.Length;
            moveDir = mgr.path[mgr.curIdx].position - mgr.Trans.position;
        }
        mgr.Trans.rotation = Quaternion.LookRotation(moveDir);
        mgr.Trans.position = mgr.Trans.position + mgr.Trans.forward * 0.015f;
    }
}
