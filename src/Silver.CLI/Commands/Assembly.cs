namespace Silver.CLI;

using Spectre.Console;
using Silver;

internal class AssemblyCmd : Runtime
{
    internal static void References(string path)
    {
        if (!File.Exists(path))
        {
            Error("The file {0} does not exist.", path);
            Program.Exit(ExitResult.NOT_FOUND);
        }
        var refs = Core.Metadata.GetReferences(path);
        Info("References:{0}", refs.Select(r => r.Name.ToString() + ": " + (r.ResolverData?.File.FullName ?? "Not resolved")));
    }
}

