using Stratis.SmartContracts;

[Deploy]

public class SimpleVerifiableContract : SmartContract
{
    public SimpleVerifiableContract(ISmartContractState state)
		:base(state)
    {
        state.PersistentState.SetAddress(nameof(Owner), state.Message.Sender);
    }

    public void Donate()
    {
        //@ assume Microsoft.Contracts.Owner.None(this.Owner);
        Address owner = Owner;
        
        //@ long oldBalance = GetBalance(owner);
        ITransferResult result = this.Transfer(owner, 10);


        //@ assert (result.Success && (GetBalance(owner) >= oldBalance)) || !result.Success;


    }

    private Address Owner
    {
        get => State.GetAddress(nameof(Owner));
            
        set => State.SetAddress(nameof(Owner), value);
    }

}
