namespace Silver.CLI.Commands;

using Silver.Core;
internal class CompilerCmd : Runtime
{
    internal static void GetProperty(string filePath, string buildConfig, string prop, params string[] additionalFiles)
    {
        Program.ExitIfFileNotFound(filePath);
        var p = Compiler.GetProperty(filePath, buildConfig, prop, additionalFiles);
        if (p is null)
        {
            Error("The property {0} does not exist or is null for the project file {1}.", prop, filePath);
            Program.Exit(ExitResult.INVALID_OPTIONS);
        }
        else
        {
            Info("The compile-time value of property {0} in build configuration {1} is {2}.", prop, buildConfig, p);
            Program.Exit(ExitResult.SUCCESS);
        }
    }
    internal static void GetCommandLine(string filePath, string buildConfig, params string[] additionalFiles)
    {
        Program.ExitIfFileNotFound(filePath);
        var cmdline = Compiler.GetCommandLine(filePath, buildConfig);
        if (cmdline is null)
        {
            Error("Could not get command line for project file {0}.", filePath);
            Program.Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            Info("Spec# compiler command-line is {0}.", "ssc " + cmdline);
            Program.Exit(ExitResult.SUCCESS);
        }
    }

    internal static void Compile(string filePath, string buildConfig, bool verify, params string[] additionalFiles)
    {
        Program.ExitIfFileNotFound(filePath);
        if (Compiler.Compile(filePath, buildConfig, verify))
        {
            Program.Exit(ExitResult.SUCCESS);
        }
        else
        {
            Program.Exit(ExitResult.UNKNOWN_ERROR);
        }
    }

}