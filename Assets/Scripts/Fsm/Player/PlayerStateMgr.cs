using UnityEngine;
using JerryFsm;

public class PlayerStateMgr : StateMgr
{
    public int curIdx;

    public Transform[] path;

    private Transform self;

    public Transform Self
    {
        get
        {
            return self;
        }
    }

    public PlayerStateMgr(Transform p, Transform[] pa)
    {
        curIdx = 0;
        self = p;
        path = pa;
    }
}

public enum PlayerStateID
{
    Walk = 0,
    Run = 1,
}
