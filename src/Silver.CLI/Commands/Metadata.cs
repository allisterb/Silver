namespace Silver.CLI.Commands;

using Silver.CLI.Core;
using static Program;
internal class AssemblyCmd : Command
{
    internal static void References(string path)
    {
        ExitIfFileNotFound(path);
        if(!Metadata.GetReferences(path))
        {
            Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            ExitWithSuccess();
        }
       
    }
}

