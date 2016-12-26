using UnityEngine;
using Jerry;

public class MonsterState_FollowPlayer : State
{
    public MonsterState_FollowPlayer(int id) : base(id) { }

    public override void Update()
    {
        base.Update();

        MonsterFsm mgr = CurFsm as MonsterFsm;

        Vector3 moveDir = mgr.Player.position - mgr.GetMgr.transform.position;
        if (moveDir.magnitude < 0.1f)
        {
            return;
        }
        mgr.GetMgr.transform.rotation = Quaternion.LookRotation(moveDir);
        mgr.GetMgr.transform.position = mgr.GetMgr.transform.position + mgr.GetMgr.transform.forward * 0.01f;
    }
}
