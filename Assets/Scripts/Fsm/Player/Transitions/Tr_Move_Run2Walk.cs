using JerryFsm;

public class Tr_Move_Run2Walk : Transition
{
    public override int NextID()
    {
        return (int)PlayerStateID.Walk;
    }

    public override bool Check()
    {
        PlayerState_Run state = m_CurState as PlayerState_Run;
        if (state.frame > 50)
        {
            return true;
        }
        return false;
    }
}
