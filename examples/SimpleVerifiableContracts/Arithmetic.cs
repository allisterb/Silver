using Stratis.SmartContracts;

public class ArithmeticContract : SmartContract
{
    public ArithmeticContract(ISmartContractState state) : base(state) {}

    #region Solidity Max example
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
    #endregion

    private uint Max(uint[] a)
    //@ requires a.Length >= 5;
    //@ ensures forall{int i in (0:a.Length); result >= a[i]};
    {
        Assert(a.Length >= 5, "The array length must be more than 5.");
        uint m = 0;
        for (int n = 0; n < a.Length; n++)
        //@ invariant forall {int i in (0:n); m >= a[i]};
        {
            if (a[n] > m)
            {
                m = a[n];
            }
        }
        return m;
    }
    public uint CallMax()
    {
        uint[] items = { 4, 5, 6, 7, 8};
        return Max(items);
    }

    #region DeepSEA Multiply example 
    /*
    object signature OS = {
        multiply : int * int -> unit
    }

    object O : OS {
        let _val : int := 0
        (* a can be negative; b must be positive *)
        let multiply (a, b) =
        for i = 0 to b do
            begin
            let val = _val in
            _val := val + a
            end;
        let val = _val in assert(val = a*b);
        _val := 0
    }
   */
    #endregion

    private int Multiply(int a, uint b)
    //@ ensures result == a * b;
    {
        int val = 0;
        for (uint i = 0; i < b; i++)
        //@ invariant i <= b;
        //@ invariant val == a * i;
        {
            val += a;
        }
        return val;
    }
    

}

