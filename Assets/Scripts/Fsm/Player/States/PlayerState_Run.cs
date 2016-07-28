using UnityEngine;
using JerryFsm;

public class PlayerState_Run : DrawNameState
{
    public int frame;

    public override void Enter()
    {
        base.Enter();
        frame = 0;
    }

    public override int ID()
    {
        return (int)PlayerStateID.Run;
    }

    public override string Name()
    {
        return "Run";
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
        fsm.Trans.position = fsm.Trans.position + fsm.Trans.forward * 0.03f;
    }
}
