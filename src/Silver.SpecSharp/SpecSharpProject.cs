namespace Silver.SpecSharp;

public readonly record struct AssemblyReference(string Name, string? ProjectPath, string? HintPath );
public abstract class SpecSharpProject : Runtime
{
    #region Constructors
    public SpecSharpProject(string fileName) :base() 
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"The project file {fileName} was not found.");
        }
        ProjectFile = new FileInfo(fileName);
    }
    #endregion

    #region Properties
    public FileInfo ProjectFile { get; init; }

    public string? AssemblyName { get; protected init; }

    public string? OutputType { get; protected set; }

    public string? RootNamespace { get; protected set; }

    public List<string> FilesToCompile { get; } = new List<string>();

    public string? StartupObject { get; protected set; }

    public string? StandardLibraryLocation { get; protected set; }

    public string? TargetPlatform { get; protected set; }

    public string? TargetPlatformLocation { get; protected set; }

    public string? ShadowedAssembly { get; protected set; }

    public AssemblyReference[]? References { get; protected set; }

    #endregion
}

