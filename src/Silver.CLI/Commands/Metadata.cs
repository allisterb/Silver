namespace Silver.CLI;

using Silver.Core;
internal class AssemblyCmd : Command
{
    internal static void References(string path)
    {
        ExitIfFileNotExists(path);
        if(!Metadata.GetReferences(path))
        {
            Program.Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            ExitWithSuccess();
        }
       
    }
}

