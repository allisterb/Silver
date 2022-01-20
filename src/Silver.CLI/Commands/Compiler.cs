namespace Silver.CLI.Commands;

using Silver.Core;
internal class CompilerCmd : Command
{
    internal static void GetProperty(string filePath, string buildConfig, string prop, params string[] additionalFiles)
    {
        Program.ExitIfFileNotFound(filePath);
        if (!Compiler.PrintProperty(filePath, buildConfig, prop, additionalFiles))
        {
            Program.Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            ExitWithSuccess();
        }
    }
    internal static void GetCommandLine(string filePath, string buildConfig, params string[] additionalFiles)
    {
        Program.ExitIfFileNotFound(filePath);
        if (!Compiler.GetCommandLine(filePath, buildConfig))
        {
            
            Program.Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            
            ExitWithSuccess();
        }
    }

    internal static void Compile(string filePath, string buildConfig, bool verify, params string[] additionalFiles)
    {
        Program.ExitIfFileNotFound(filePath);
        if (!Compiler.Compile(filePath, buildConfig, verify))
        {
            Program.Exit(ExitResult.UNKNOWN_ERROR);
        }
        else
        {
            ExitWithSuccess();
        }
    }

}