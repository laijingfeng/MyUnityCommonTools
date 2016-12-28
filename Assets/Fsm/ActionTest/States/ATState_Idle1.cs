using UnityEngine;
using Jerry;

public class ATState_Idle1 : State
{
    public ATState_Idle1(int id) : base(id) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("ATState_Idle1 Enter");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("ATState_Idle1 Exit");
    }
}