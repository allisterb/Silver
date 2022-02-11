using Stratis.SmartContracts;

public class We : SmartContract
{
    public We(ISmartContractState state) : base(state) { }
    public int Add(int a, int b)
    {
        var x = new Address[4];
        var gg = x[0].GetType();
        
        int[] ii = { 1, 2, 3, 4, };
        var ll = ii.GetEnumerator();
        
        return a + b; 
    }

    //private int testField = 0;
}
