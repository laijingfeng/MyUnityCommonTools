using UnityEngine;
using Jerry;

public class MonsterFsm : Fsm
{
    private Transform player;

    public Transform Player
    {
        get
        {
            return player;
        }
    }

    public MonsterFsm(Transform p)
    {
        player = p;
    }
}

public enum MonsterStateID
{
    Idle = 0,
    FollowPlayer = 1,
    RunWay = 2,
}