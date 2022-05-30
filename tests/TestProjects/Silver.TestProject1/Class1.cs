using Stratis.SmartContracts;

public class We : SmartContract
{
    public We(ISmartContractState state) : base(state) { }
    public int Add(int a, int b)
    {
        var x = new Address[4];
        //var gg = x[0].GetType();
        
        int[] ii = { 1, 2, 3, 4, };
        //var ll = ii.GetEnumerator();

        var ss = new TestStruct()
        {
            Name = "ll",
            Time = 0
        };

        return a + b;

        
    }


    //private int testField = 0;
    #region Structs


    public struct TestStruct
    {
        public TestStruct(string name, ulong time)
        {
            this.Name = name;
            this.Time = time;
        }
        public string Name;

        public ulong Time;

    }
    #endregion
}


