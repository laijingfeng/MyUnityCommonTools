using UnityEngine;
using Jerry;

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

        MonsterFsm mgr = CurFsm as MonsterFsm;

        Vector3 moveDir = path[curIdx].position - mgr.GetMgr.transform.position;
        if (moveDir.magnitude < 0.1f)
        {
            curIdx = (curIdx + 1) % path.Length;
            moveDir = path[curIdx].position - mgr.GetMgr.transform.position;
        }
        mgr.GetMgr.transform.rotation = Quaternion.LookRotation(moveDir);
        mgr.GetMgr.transform.position = mgr.GetMgr.transform.position + mgr.GetMgr.transform.forward * 0.01f;
    }
}