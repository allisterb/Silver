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
    //@ ensures this.Balances[Owner] != old(this.Balances[Owner]) + 10;
    

    {
        //@ expose (this) {
        //@ assume this.Balances.ContainsKey(Owner);
        
        this.Transfer(Owner, 10);
        
        //@ }

    }

    private Address Owner
    {
        get
        {
            //@ expose(this) {
            return State.GetAddress(nameof(Owner));
            //@ }
        }
        set => State.SetAddress(nameof(Owner), value);
    }
}
