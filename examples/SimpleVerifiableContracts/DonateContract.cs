using Stratis.SmartContracts;

public class DonateContract : SmartContract
{
    public DonateContract(ISmartContractState state)
        : base(state)
    {
        state.PersistentState.SetAddress(nameof(Owner), state.Message.Sender);
        
    }

    public ITransferResult Donate()
    //@ ensures result.Success ==> GetBalance(Owner) == old(GetBalance(Owner)) + Message.Value;
    {
        //@ assume Microsoft.Contracts.Owner.Same(this, Owner);
        //@ ulong oldBalance = GetBalance(Owner);
        ITransferResult result = Transfer(Owner, Message.Value);
        //@ assert result.Success ==> GetBalance(Owner) == oldBalance + Message.Value;
        return result;
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
