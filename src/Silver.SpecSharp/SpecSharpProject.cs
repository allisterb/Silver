namespace Silver.SpecSharp;

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

    public string AssemblyName { get; protected init; } = string.Empty;

    public string OutputType { get; protected set; } = string.Empty;

    public string RootNamespace { get; protected set; } = string.Empty;

    public List<string> FilesToCompile { get; } = new List<string>();

    public string StartupObject { get; protected set; } = string.Empty;

    public string StandardLibraryLocation { get; protected set; } = string.Empty;

    public string TargetPlatform { get; protected set; } = string.Empty;

    public string TargetPlatformLocation { get; protected set; } = string.Empty;

    #endregion
}

