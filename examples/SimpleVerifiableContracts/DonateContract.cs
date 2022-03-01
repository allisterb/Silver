using Stratis.SmartContracts;

public class DonateContract : SmartContract
{
    public DonateContract(ISmartContractState state)
        : base(state)
    {
        state.PersistentState.SetAddress(nameof(Owner), state.Message.Sender);
    }

    public void Donate()
    {
        //@ assume Microsoft.Contracts.Owner.Same(Message, Owner);
        //@ ulong oldBalance = GetBalance(Owner);
        Transfer(Owner, Message.Value);
        //@ assert GetBalance(Owner) == oldBalance + Message.Value;
    }

    private Address Owner
    {
        get
        {
            //@ assume Microsoft.Contracts.Owner.Same(State, Owner);
            return State.GetAddress(nameof(Owner));
        }
        set
        {
            //@ assume Microsoft.Contracts.Owner.Same(State, Owner);
            State.SetAddress(nameof(Owner), value);
        }
    }
}
