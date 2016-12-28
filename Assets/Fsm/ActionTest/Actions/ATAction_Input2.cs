using UnityEngine;
using Jerry;

public class ATAction_Input2 : Action
{
    public override void Enter()
    {
        base.Enter();
        Debug.Log("ATAction_Input2 Enter");
    }

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.N))
        {
            CurState.CurFsm.ChangeState(ATStateID.Idle1.GetHashCode());
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("ATAction_Input2 Exit");
    }
}