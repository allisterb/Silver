using Stratis.SmartContracts;

//@ using Microsoft.Contracts;
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
        this.Transfer(Owner, 10);
        
       

    }

    private Address Owner
    
    {
        get
        {
            
            return State.GetAddress(nameof(Owner));
            
        }
        set => State.SetAddress(nameof(Owner), value);
    }
}
