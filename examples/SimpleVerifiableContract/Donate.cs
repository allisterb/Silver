using Stratis.SmartContracts;

//@ using Microsoft.Contracts;
[Deploy]

public class SimpleVerifiableContract : SmartContract
{
    //@ [NotDelayed]
    public SimpleVerifiableContract(ISmartContractState state)
		:base(state)
    {
        state.PersistentState.SetAddress(nameof(Owner), state.Message.Sender);

    }

    public void Donate()
    {
        ITransferResult result = Transfer(Owner, Message.Value);
        Assert(result.Success, "Transfer failed.");
    }

   

    
    private Address Owner
    {
        get => State.GetAddress(nameof(Owner));
    }
}
