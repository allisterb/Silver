using Stratis.SmartContracts;

[Deploy]
public class ArithmeticContract : SmartContract
{
    public ArithmeticContract(ISmartContractState state) : base(state) {}

    public uint Max(uint[] a)
    //@ ensures forall{int i in (0:a.Length); result >= a[i]};
    {
        uint m = uint.MinValue;
        for (int n = 0; n < a.Length; n++)
        //@ invariant n <= a.Length;
        //@ invariant forall {int i in (0:n); m >= a[i]};
        {
            if (a[n] > m)
            {
                m = a[n];
            }
        }
        return m;
    }
    public uint TestMax()
    {
        uint[] items = { 4, 5, 6, 7, 8 };
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

