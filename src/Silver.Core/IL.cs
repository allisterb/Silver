using Silver.CodeAnalysis.IL;

namespace Silver.Core
{
    public class IL : Runtime
    {
        public static void Test()
        {
            var c = new Class1();
            c.CanAnalyzeClassHierarchy();
        }

        public static string? Disassemble(string fileName) => Disassembler.Print(FailIfFileNotFound(fileName));
    }
}
