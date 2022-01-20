namespace Silver.CLI.Commands;

using Silver.CLI.Core;
using static Program;
internal class ILCmd : Command
{
    internal static void Dissassemble(string fileName, bool boogie, bool noIL, bool noStack)
    {
        ExitIfFileNotExists(fileName);
        if (!IL.Disassemble(fileName, boogie, noIL, noStack))
        {
            Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            ExitWithSuccess();
        }
    }

    internal static void Summarize(string fileName, bool all)
    {
        ExitIfFileNotExists(fileName);
        if (!IL.Summarize(fileName, all))
        {
            Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            ExitWithSuccess();
        }
    }

    internal static void PrintCallGraph(string fileName, bool all)
    {
        ExitIfFileNotExists(fileName);
        IL.PrintCallGraphs(fileName, all);
    }
}

