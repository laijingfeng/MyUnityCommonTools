using UnityEngine;
using JerryFsm;

public class MonsterStateMgr : StateMgr
{
    private Transform player;
    private Transform self;

    public Transform Player
    {
        get
        {
            return player;
        }
    }

    public Transform Self
    {
        get
        {
            return self;
        }
    }

    public MonsterStateMgr(Transform p, Transform s)
    {
        player = p;
        self = s;
    }
}

public enum MonsterStateID
{
    Idle = 0,
    FollowPlayer = 1,
    RunWay = 2,
}