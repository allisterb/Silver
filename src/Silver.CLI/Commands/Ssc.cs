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
        SpecSharp.Compile(fileName);
    }
}

