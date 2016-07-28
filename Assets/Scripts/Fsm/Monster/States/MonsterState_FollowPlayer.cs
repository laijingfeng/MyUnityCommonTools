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

        MonsterStateMgr mgr = m_StateMgr as MonsterStateMgr;

        Vector3 moveDir = mgr.Player.position - mgr.Self.position;
        if (moveDir.magnitude < 0.1f)
        {
            return;
        }
        mgr.Self.rotation = Quaternion.LookRotation(moveDir);
        mgr.Self.position = mgr.Self.position + mgr.Self.forward * 0.01f;
    }
}
