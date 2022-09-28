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
        //@ assume Microsoft.Contracts.Owner.None(Owner);
		//@ ulong oldBalance = GetBalance(Owner);
        ITransferResult r = Transfer(Owner, Message.Value);
        //@ assert (GetBalance(Owner) == oldBalance + Message.Value);
    }

    private Address Owner
    {
        get => State.GetAddress(nameof(Owner));
        set => State.SetAddress(nameof(Owner), value);
    }
}
