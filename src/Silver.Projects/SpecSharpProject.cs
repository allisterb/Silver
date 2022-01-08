namespace Silver.Projects;

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
        TargetPlatform = "v4";
    }
    #endregion

    #region Properties
    public FileInfo ProjectFile { get; init; }

    public string AssemblyName { get; protected set; } = string.Empty;

    public string DefineConstants { get; protected set; } = string.Empty;

    public string OutputType { get; protected set; } = string.Empty;

    public bool DebugEnabled { get; protected set; } = false;

    public string RootNamespace { get; protected set; } = string.Empty;

    public List<string> SourceFiles { get; init; } = new();

    public string TargetPath { get; protected set; } = string.Empty;

    public string StartupObject { get; protected set; } = string.Empty;

    public string StandardLibraryLocation { get; protected set; } = string.Empty;

    public string TargetPlatform { get; protected set; } = string.Empty;

    public string TargetPlatformLocation { get; protected set; } = string.Empty;

    public string ShadowedAssembly { get; protected set; } = string.Empty;

    public List<string> References { get; protected set; } = new();

    public bool NoStdLib { get; protected set; } = false;

    #endregion

    #region Methods
    public string CommandLine
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(TargetPath))
            {
                sb.AppendFormat("/out:{0} ", TargetPath);
            }
            if (!string.IsNullOrEmpty(OutputType))
            {
                sb.AppendFormat("/target:{0} ", OutputType);
            }
            if (!string.IsNullOrEmpty(DefineConstants))
            {
                sb.AppendFormat("/define:{0} ", DefineConstants);
            }
            if (DebugEnabled)
            {
                sb.Append("/debug+ /debug:pdbonly ");
            }
            if (NoStdLib)
            {
                sb.Append("/nostdlib+ ");
            }
            return sb.ToString();
        }
    }
    #endregion
}