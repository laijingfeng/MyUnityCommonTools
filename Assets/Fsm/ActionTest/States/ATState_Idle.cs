using UnityEngine;
using Jerry;

public class ATState_Idle : State
{
    public ATState_Idle(int id) : base(id){}

    public override void Enter()
    {
        base.Enter();
        Debug.Log("ATState_Idle Enter");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("ATState_Idle Exit");
    }
}