using Jerry;

public class Tr_Move_Walk2Run : Transition
{
    public Tr_Move_Walk2Run(int nextID) : base(nextID) { }

    public override bool Check()
    {
        PlayerState_Walk state = CurState as PlayerState_Walk;
        if (state.frame > 50)
        {
            return true;
        }
        return false;
    }
}