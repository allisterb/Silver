using System;
using Stratis.SmartContracts;

//@ using Microsoft.Contracts;

[Deploy]
public class ArithmeticContract : SmartContract
{
    public ArithmeticContract(ISmartContractState state) : base(state) {}

    public uint Max(uint[] _a)
    //@ requires _a.Length >= 5;
    {
        Assert(_a.GetLength(0) >= 5, "");
        uint m = 0;
        for (uint i = 0; i < _a.GetLength(0); ++i)
        {
            if (_a[i] > m)
            {
                m = _a[i];
            }
        }
        return m;

    }

    public uint Test1()
    {
        uint[] items = { 4, 5, 6 };
        return Max(items);
    }
    /*
    contract Max
    {
        function max(uint[] memory _a) public pure returns(uint)
    {
        require(_a.length >= 5);
        uint m = 0;
        for (uint i = 0; i < _a.length; ++i)
            if (_a[i] > m)
                m = _a[i];

        for (uint i = 0; i < _a.length; ++i)
            assert(m > _a[i]);

        return m;
    }
    */

}

