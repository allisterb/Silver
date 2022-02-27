using Stratis.SmartContracts;

[Deploy]

public class SimpleVerifiableContract : SmartContract
{
    public SimpleVerifiableContract(ISmartContractState state)
        : base(state)
    {
        state.PersistentState.SetAddress(nameof(Owner), state.Message.Sender);
    }

    public void Donate()
    {
        //@ assume Microsoft.Contracts.Owner.Same(this, this.Owner);
        //@ Address owner = Owner;
        //@ ulong oldBalance = GetBalance(Owner);
        //@ ulong credit = Message.Value;

        ITransferResult result = this.Transfer(Owner, Message.Value);
        Assert(result.Success, "ll");

        //@ assert GetBalance(owner) == oldBalance + credit;
    }

    private Address Owner
    {
        //@ 
        get => State.GetAddress(nameof(Owner));
        set => State.SetAddress(nameof(Owner), value);   
    }
}
