﻿using Stratis.SmartContracts;

public class DonateContract : SmartContract
{
    public DonateContract(ISmartContractState state)
        : base(state)
    {
        state.PersistentState.SetAddress(nameof(Owner), state.Message.Sender);
    }

    public void Donate()
    //@ ensures GetBalance(Owner) == old(GetBalance(Owner)) + Message.Value;
    {
        //@ assume Microsoft.Contracts.Owner.Same(Owner, this);
        
        //Owner = Address.Zero;
        //@ assert Owner == Address.Zero;
        ITransferResult r = Transfer(Owner, Message.Value);
        Assert(r.Success, "The transfer did not succeed.") ;
    }

    private Address Owner
    {
        get => State.GetAddress(nameof(Owner));
        set => State.SetAddress(nameof(Owner), value);
    }
}
