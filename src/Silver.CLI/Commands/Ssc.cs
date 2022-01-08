namespace Silver.CLI.Commands;

using Silver.Core;
internal class SscCmd : Runtime
{
    internal static void Compile(string filePath, params string[] args)
    {
        Program.ExitIfFileNotFound(filePath);
        Projects.Compile(filePath);
    }

    internal static void GetProperty(string filePath, string prop)
    {
        Program.ExitIfFileNotFound(filePath);
        var p = Projects.GetProperty(filePath, prop);
        if (p is null)
        {
            Error("The property {0} does not exist for the project file {1}.", prop, filePath);
            Program.Exit(ExitResult.INVALID_OPTIONS);
        }
        else
        {
            Info("The compile-time value of property {0} is {1}.", prop, p);
            Program.Exit(ExitResult.SUCCESS);
        }
    }
    internal static void GetCommandLine(string filePath)
    {
        Program.ExitIfFileNotFound(filePath);
        var cmdline = Projects.GetCommandLine(filePath);
        if (cmdline is null)
        {
            Error("Could not get command line for project file {0}.", filePath);
            Program.Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            Info("Spec# compiler command-line is {0}.", cmdline);
            Program.Exit(ExitResult.SUCCESS);
        }
    }
}