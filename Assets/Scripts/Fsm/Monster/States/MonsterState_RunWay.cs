using UnityEngine;
using JerryFsm;

public class MonsterState_RunWay : State
{
    private int curIdx;
    private Transform[] path;

    public MonsterState_RunWay(int id, Transform[] p)
        : base(id)
    {
        path = p;
        curIdx = 0;
    }

    public override void Update()
    {
        base.Update();

        if (path == null || path.Length <= 0)
        {
            return;
        }

        MonsterStateMgr mgr = m_StateMgr as MonsterStateMgr;

        Vector3 moveDir = path[curIdx].position - mgr.Self.position;
        if (moveDir.magnitude < 0.1f)
        {
            curIdx = (curIdx + 1) % path.Length;
            moveDir = path[curIdx].position - mgr.Self.position;
        }
        mgr.Self.rotation = Quaternion.LookRotation(moveDir);
        mgr.Self.position = mgr.Self.position + mgr.Self.forward * 0.01f;
    }
}
