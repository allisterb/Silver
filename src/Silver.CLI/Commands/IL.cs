namespace Silver.CLI.Commands;

using Silver.CLI.Core;
using static Program;
internal class ILCmd : Command
{
    internal static void Dissassemble(string fileName, bool boogie, bool noIL, bool noStack, bool all)
    {
        ExitIfFileNotExists(fileName);
        if (!IL.Disassemble(fileName, boogie, noIL, noStack, all))
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

    internal static void PrintCallGraph(CallGraphOptions o)
    {
        ExitIfFileNotExists(o.InputFile);
        IL.PrintCallGraphs(o.InputFile, o.OutputFile, o.OutputFormat, o.AllClasses);
    }

    internal static void PrintControlFlowGraph(ControlFlowGraphOptions o)
    {
        ExitIfFileNotExists(o.InputFile);
        if (!Enum.TryParse<Drawing.GraphFormat>(o.OutputFormat.ToUpper(), out var graphFormat))
        {
            Error("Invalid graph format: {0}.", o.OutputFormat);
            Exit(ExitResult.INVALID_OPTIONS);
        }
        IL.PrintControlFlowGraph(o.InputFile, o.OutputFile, o.OutputFormat, o.AllClasses);
    }
}

