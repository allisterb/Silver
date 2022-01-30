﻿using Stratis.SmartContracts;
//@ using Microsoft.Contracts;

[Deploy]
public class MultiplyContract : SmartContract
{
	public MultiplyContract(ISmartContractState state, string message)
		: base(state) {}

	public int Multiply(int x, int y)
    //@  requires 0 <= y;
    //@ requires 0 <= x;
    //@ ensures result == y * x;
        {
            int q = 0;
            int i = 0;
            while (i <= y)
            //@ invariant 0<= i;
            //@ invariant i<= y;
            //@ invariant q == (i * x);
            {

                //@ assert  q == (i * x);
                //@ assert  q + x == (i * x) + x;
                q = q + x;

                //@ assert  q == (i * x) + x;
                //assumption needed for distribution of operators.
                //@ assume (i*x) + x == (i+1) * x;
                //@ assert  q == (i+1) * x;
                i = i + 1;
                //@ assert  q == i * x;

            }

            //@ assert  q == i * x && i == y;
            return q;
        }
}
