namespace Silver.CLI.Commands;

using Microsoft.Msagl.Drawing;

using Silver.CLI.Core;
using static Program;
internal class ILCmd : Command
{
    internal static void Dissassemble(string fileName, bool boogie, bool noIL, string? classPattern = null, string? methodPattern = null)
    {
        ExitIfFileNotFound(fileName);
        if (!IL.Disassemble(fileName, boogie, noIL, classPattern, methodPattern))
        {
            Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            ExitWithSuccess();
        }
    }

    internal static void Summarize(string fileName)
    {
        ExitIfFileNotFound(fileName);
        if (!IL.Summarize(fileName))
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
        ExitIfFileNotFound(o.InputFile);
        IL.PrintCallGraphs(o.InputFile, o.OutputFile, o.OutputFormat);
    }

    internal static void PrintControlFlowGraph(ControlFlowGraphOptions o)
    {
        ExitIfFileNotFound(o.InputFile);
        if (!Enum.TryParse<GraphFormat>(o.OutputFormat.ToUpper(), out var graphFormat))
        {
            Error("Invalid graph format: {0}.", o.OutputFormat);
            Exit(ExitResult.INVALID_OPTIONS);
        }
        IL.PrintControlFlowGraph(o.InputFile, o.OutputFile, o.OutputFormat);
    }
}

