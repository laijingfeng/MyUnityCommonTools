using UnityEngine;
using JerryFsm;

public class MonsterState_RunWay : State
{
    private int curIdx;
    private Transform[] path;

    public MonsterState_RunWay(Transform[] p)
    {
        path = p;
        curIdx = 0;
    }

    public override int ID()
    {
        return (int)MonsterStateID.RunWay;
    }

    public override void Update()
    {
        base.Update();

        if (path == null || path.Length <= 0)
        {
            return;
        }

        MonsterFsm mgr = m_StateMgr as MonsterFsm;

        Vector3 moveDir = path[curIdx].position - mgr.Trans.position;
        if (moveDir.magnitude < 0.1f)
        {
            curIdx = (curIdx + 1) % path.Length;
            moveDir = path[curIdx].position - mgr.Trans.position;
        }
        mgr.Trans.rotation = Quaternion.LookRotation(moveDir);
        mgr.Trans.position = mgr.Trans.position + mgr.Trans.forward * 0.01f;
    }
}
