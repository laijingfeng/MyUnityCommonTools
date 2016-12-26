using Jerry;
using UnityEngine;
using System.Collections;

public class MonsterState_Idle : State
{
    public MonsterState_Idle(int id) : base(id) { }

    public override void Enter()
    {
        base.Enter();
        //CurFsm.GetMgr.StartCoroutine("IE_Idle");
    }

    public override void Exit()
    {
        base.Exit();
        //CurFsm.GetMgr.StopCoroutine("IE_Idle");
    }

    public IEnumerator IE_Idle()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("idle");
    }
}