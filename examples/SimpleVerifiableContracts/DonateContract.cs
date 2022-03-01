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
        //@ assume Microsoft.Contracts.Owner.Same(this, Owner);
        //@ ulong oldBalance = GetBalance(Owner);
        //@ ulong credit = Message.Value;
        //@ ITransferResult result = this.Transfer(Owner, credit);
        //@ assert result.Success ==> GetBalance(Owner) == oldBalance + credit;
    }

    private /*@ static @*/ Address Owner
    {
        get
        {
            //@ assume Microsoft.Contracts.Owner.Same(State, Owner);
            return State.GetAddress(nameof(Owner));
        }
    }

}
