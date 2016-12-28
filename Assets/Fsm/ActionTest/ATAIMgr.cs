using Jerry;

public class ATAIMgr : AIMgr
{
    public override void Start()
    {
        base.Start();
        StartFsm();
    }

    public override void MakeFsm()
    {
        m_Fsm = new ATFsm();

        ATState_Idle idle = new ATState_Idle(ATStateID.Idle.GetHashCode());
        idle.SetSequnceAction(true);
        idle.AddAction(new ATAction_Input());
        idle.AddAction(new ATAction_Input2());
        m_Fsm.AddState(idle);

        ATState_Idle1 idle1 = new ATState_Idle1(ATStateID.Idle1.GetHashCode());
        m_Fsm.AddState(idle1);
    }
}