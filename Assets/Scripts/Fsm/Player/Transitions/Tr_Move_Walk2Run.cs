using JerryFsm;

public class Tr_Move_Walk2Run : Transition
{
    public Tr_Move_Walk2Run(int nID)
        : base(nID)
    {

    }

    public override bool Check()
    {
        PlayerState_Walk state = m_CurState as PlayerState_Walk;
        if (state.frame > 50)
        {
            return true;
        }
        return base.Check();
    }
}
