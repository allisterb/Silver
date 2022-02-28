using Stratis.SmartContracts;

[Deploy]
public class ArithmeticContract : SmartContract
{
    public ArithmeticContract(ISmartContractState state) : base(state) {}

    uint Max(uint[] _a)
    //@ requires _a.Length >= 5;
    
    {
        uint m = 0;
        for (int i = 0; i < _a.Length; ++i)
        {
            if (_a[i] > m)
            {
                m = _a[i];
            }
        }
        // assert forall {int i in (0:_a.Length); (m >= _a[i])}; 
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

