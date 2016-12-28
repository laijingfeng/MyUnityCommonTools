using UnityEngine;
using Jerry;

public class ATAction_Input : Action
{
    public override void Enter()
    {
        base.Enter();
        Debug.Log("ATAction_Input Enter");
    }

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.M))
        {
            Finish();
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("ATAction_Input Exit");
    }
}