namespace Silver.CLI.Commands;

using Silver.Core;
internal class ILCmd : Command
{
    internal static void Dissassemble(string fileName, bool boogie, bool noIL, bool noStack)
    {
        ExitIfFileNotExists(fileName);
        var d = IL.Disassemble(fileName, boogie, noIL, noStack);
       if (d is null)
       {
            Error("Could not disassemble {0}.", fileName);
            Program.Exit(ExitResult.ERROR_IN_RESULTS);
       }
       else
        {
            Con.WriteLine(d);
            ExitWithSuccess();
        }
    }

    internal static void Summarize(string fileName, bool all)
    {
        ExitIfFileNotExists(fileName);
        IL.Summarize(fileName, all);
    }

    internal static void PrintCFG(string fileName, bool all)
    {
        ExitIfFileNotExists(fileName);
        IL.Summarize(fileName, all);
    }
}

