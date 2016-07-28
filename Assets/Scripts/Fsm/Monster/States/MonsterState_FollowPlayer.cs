using UnityEngine;
using JerryFsm;

public class MonsterState_FollowPlayer : State
{
    public override int ID()
    {
        return (int)MonsterStateID.FollowPlayer;
    }

    public override void Update()
    {
        base.Update();

        MonsterFsm mgr = m_StateMgr as MonsterFsm;

        Vector3 moveDir = mgr.Player.position - mgr.Trans.position;
        if (moveDir.magnitude < 0.1f)
        {
            return;
        }
        mgr.Trans.rotation = Quaternion.LookRotation(moveDir);
        mgr.Trans.position = mgr.Trans.position + mgr.Trans.forward * 0.01f;
    }
}
