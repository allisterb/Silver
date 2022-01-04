namespace Silver.Core;

using Silver.SpecSharp;
public class SpecSharp : Runtime
{
    public static void Compile(string file)
    {
        FileInfo f = new FileInfo(file);

        SpecSharpProject project = f.Extension == ".csproj" ? new CsSpecSharpProject(file) : new XmlSpecSharpProject(file);
    }
    
}

