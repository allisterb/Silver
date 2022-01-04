namespace Silver.Core;

using Silver.Metadata;
public class Metadata : Runtime
{
    public static IEnumerable<AssemblyReference> GetReferences(string path) => GetTimed(() => new Assembly(path).References, "Loading assembly", "Loading assembly {0}", path);
    
}

