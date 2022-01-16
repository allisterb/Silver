namespace Silver.CLI.Commands;

using Silver.Core;
internal class ILCmd : Command
{
    internal static void Dissassemble(string fileName)
    {
       ExitIfFileNotExists(fileName);
       if (!IL.PrintDisassembly(fileName))
       {
            Program.Exit(ExitResult.ERROR_IN_RESULTS);
       }
       else
        {
            ExitWithSuccess();
        }
    }
}

