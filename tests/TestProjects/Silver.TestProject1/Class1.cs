using Stratis.SmartContracts;

public class We : SmartContract
{
    public We(ISmartContractState state) : base(state) { }
    public int Add(int a, int b)
    {
        var x = new Address[4];
        //var xx = new System.Collections.Generic.List<string> { "foo" };
        //var tt = new string("h".ToCharArray());
        //var len = tt.Length;
        int[] ii = { 1, 2, 3, 4, };
        var ll = ii.GetEnumerator();
        return a + b; 
    }

    //private int testField = 0;
}
