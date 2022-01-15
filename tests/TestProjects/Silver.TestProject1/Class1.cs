namespace Silver.TestProject1
{
    
    public class Class1 : Stratis.SmartContracts.SmartContractAssertException
    {
        public Class1(string msg) : base(msg) { }
        public int Add(int a, int b)
        { 
            return a + b; 
        }
    }
}