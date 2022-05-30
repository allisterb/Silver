namespace Silver.CLI.Core;

using Silver;
using Silver.Metadata;
public class Metadata : Runtime
{
    public static bool GetReferences(string path)
    {
        var refs = Program.GetTimed_(() => new AssemblyMetadata(path).References, "Loading assembly", "Loading assembly {0}", path);
        if (refs is null)
        {
            Error("Could not get references for assembly {0}.", path);
            return false;
        }
        else
        {
            Info("References:{0}", refs.Select(r => r.Name.ToString() + ": " + (r.ResolverData?.File.FullName ?? "Not resolved")));
            return true;
        }
    }
    
}

