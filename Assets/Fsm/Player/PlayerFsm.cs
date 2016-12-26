using UnityEngine;
using Jerry;

public class PlayerFsm : Fsm
{
    public int curIdx;

    public Transform[] path;

    public PlayerFsm(Transform[] pa)
    {
        curIdx = 0;
        path = pa;
    }
}

public enum PlayerStateID
{
    Walk = 0,
    Run = 1,
}