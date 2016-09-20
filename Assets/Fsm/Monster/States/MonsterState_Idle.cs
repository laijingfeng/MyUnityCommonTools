using JerryFsm;

public class MonsterState_Idle : State
{
    public override int ID()
    {
        return (int)MonsterStateID.Idle;
    }

    public override void Update()
    {
        base.Update();
    }
}
