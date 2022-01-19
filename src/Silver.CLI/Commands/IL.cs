namespace Silver.CLI.Commands;

using Silver.Core;
internal class ILCmd : Command
{
    internal static void Dissassemble(string fileName, bool boogie, bool noIL, bool noStack)
    {
        ExitIfFileNotExists(fileName);
        if (!IL.Disassemble(fileName, boogie, noIL, noStack))
        {
            Program.Exit(ExitResult.ERROR_IN_RESULTS);
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
            Program.Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            ExitWithSuccess();
        }
    }

    internal static void PrintCFG(string fileName, bool all)
    {
        ExitIfFileNotExists(fileName);
        IL.Summarize(fileName, all);
    }
}

