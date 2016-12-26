using UnityEngine;
using Jerry;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerState_Walk : State
{
    public int frame;

    public PlayerState_Walk(int id) : base(id) { }

    public override void Enter()
    {
        base.Enter();
        frame = 0;
    }

    public override void Draw()
    {
        base.Draw();
#if UNITY_EDITOR
        Handles.Label(CurFsm.GetMgr.transform.position, "Walk");
#endif
    }

    public override void Update()
    {
        base.Update();

        frame++;

        PlayerFsm fsm = CurFsm as PlayerFsm;

        if (fsm.path == null || fsm.path.Length <= 0)
        {
            return;
        }

        Vector3 moveDir = fsm.path[fsm.curIdx].position - fsm.GetMgr.transform.position;
        if (moveDir.magnitude < 0.1f)
        {
            fsm.curIdx = (fsm.curIdx + 1) % fsm.path.Length;
            moveDir = fsm.path[fsm.curIdx].position - fsm.GetMgr.transform.position;
        }
        fsm.GetMgr.transform.rotation = Quaternion.LookRotation(moveDir);
        fsm.GetMgr.transform.position = fsm.GetMgr.transform.position + fsm.GetMgr.transform.forward * 0.015f;
    }
}