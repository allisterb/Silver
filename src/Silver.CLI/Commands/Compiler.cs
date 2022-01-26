namespace Silver.CLI.Commands;

using Silver.CLI.Core;
using static Program;

internal class CompilerCmd : Command
{
    internal static void GetProperty(string filePath, string buildConfig, string prop, params string[] additionalFiles)
    {
        ExitIfFileNotFound(filePath);
        if (!Compiler.PrintProperty(filePath, buildConfig, prop, additionalFiles))
        {
            Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            ExitWithSuccess();
        }
    }
    internal static void GetCommandLine(string filePath, string buildConfig, params string[] additionalFiles)
    {
        ExitIfFileNotFound(filePath);
        if (!Compiler.GetCommandLine(filePath, buildConfig))
        {
            
            Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            
            ExitWithSuccess();
        }
    }

    internal static void Compile(string filePath, string buildConfig, bool verify, bool ssc, params string[] additionalFiles)
    {
        ExitIfFileNotFound(filePath);
        if (!Compiler.Compile(filePath, buildConfig, verify, ssc))
        {
            Exit(ExitResult.UNKNOWN_ERROR);
        }
        else
        {
            ExitWithSuccess();
        }
    }

}