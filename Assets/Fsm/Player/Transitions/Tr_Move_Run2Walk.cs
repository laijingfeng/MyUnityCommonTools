using Jerry;

public class Tr_Move_Run2Walk : Transition
{
    public Tr_Move_Run2Walk(int nextID) : base(nextID) { }

    public override bool Check()
    {
        PlayerState_Run state = CurState as PlayerState_Run;
        if (state.frame > 50)
        {
            return true;
        }
        return false;
    }
}