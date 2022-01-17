namespace Silver.CLI.Commands;

using Silver.Core;
internal class ILCmd : Command
{
    internal static void Dissassemble(string fileName, bool boogie, bool noIL, bool noStack)
    {
       ExitIfFileNotExists(fileName);
       if (!IL.PrintDisassembly(fileName, boogie, noIL, noStack))
       {
            Program.Exit(ExitResult.ERROR_IN_RESULTS);
       }
       else
        {
            ExitWithSuccess();
        }
    }
}

