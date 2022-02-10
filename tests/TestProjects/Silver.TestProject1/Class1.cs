using Stratis.SmartContracts;

public class We : SmartContract
{
    public We(ISmartContractState state) : base(state) { }
    public int Add(int a, int b)
    {
        var x = new Address[4];
        //var xx = new System.Collections.Generic.List<string> { "foo" };
        var tt = new string("h".ToCharArray());
        var len = tt.Length;
        return a + b; 
    }

    //private int testField = 0;
}
