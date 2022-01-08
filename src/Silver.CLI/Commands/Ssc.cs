namespace Silver.CLI.Commands;

using Silver.Core;
internal class SscCmd : Runtime
{
    internal static void Compile(string fileName, params string [] args)
    {
        if (!File.Exists(fileName))
        {
            Error("The file {0} does not exist.", fileName);
            Program.Exit(ExitResult.NOT_FOUND);
        }
        Projects.Compile(fileName);
    }

    internal static void GetProperty(string filePath, string prop)
    {
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
}

