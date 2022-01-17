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

        public static bool PrintDisassembly(string fileName, bool boogie, bool noIL, bool noStack)
        {
            var output = boogie ? Translator.ToBoogie(FailIfFileNotFound(fileName)) : Disassembler.Run(FailIfFileNotFound(fileName), noIL, noStack);
            if (output is null)
            {
                Error("Could not disassemble {0}.", fileName);
                return false;
            }
            else
            {
                Con.WriteLine(output);
                return true;
            }
        }
    }
}
