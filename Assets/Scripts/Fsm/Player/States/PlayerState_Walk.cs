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
        Handles.Label(m_Fsm.Trans.position, "Walk");
#endif
    }

    public override void Update()
    {
        base.Update();
        
        frame++;

        PlayerFsm fsm = m_Fsm as PlayerFsm;

        if (fsm.path == null || fsm.path.Length <= 0)
        {
            return;
        }

        Vector3 moveDir = fsm.path[fsm.curIdx].position - fsm.Trans.position;
        if (moveDir.magnitude < 0.1f)
        {
            fsm.curIdx = (fsm.curIdx + 1) % fsm.path.Length;
            moveDir = fsm.path[fsm.curIdx].position - fsm.Trans.position;
        }
        fsm.Trans.rotation = Quaternion.LookRotation(moveDir);
        fsm.Trans.position = fsm.Trans.position + fsm.Trans.forward * 0.015f;
    }
}
