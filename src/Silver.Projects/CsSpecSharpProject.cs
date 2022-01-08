namespace Silver.Projects;

using System.Reflection;

using Buildalyzer;

using Silver.Metadata;

public class CsSpecSharpProject : SpecSharpProject
{
    #region Constructors
    static CsSpecSharpProject() { }
    public CsSpecSharpProject(string filePath, CsSpecSharpProject? parent = null) : base(filePath)
    {
        using (var op = Begin("Loading MSBuild project {0}", filePath))
        {
            StringWriter log = new StringWriter();
            AnalyzerManagerOptions options = new AnalyzerManagerOptions
            {
                LogWriter = log
            };
            var m = new AnalyzerManager(options);
            var a = m.GetProject(filePath);
            Debug("Performing a design-time build of {0}.", ProjectFile.FullName);
            var _results = a.Build();
            foreach (var l in log.ToString().Split(Environment.NewLine))
            {
                Debug(l);
            }
            if (!_results.Any(r => r.Succeeded))
            {
                Error("Design-time build of {0} failed.", ProjectFile.FullName);
            }
            MsBuildProject = _results.First();
            BuildConfiguration = MsBuildProject.GetProperty("Configuration");
            DefineConstants = MsBuildProject.GetProperty("DefineConstants");
            AssemblyName = MsBuildProject.GetProperty("AssemblyName");
            OutputType = MsBuildProject.GetProperty("OutputType").ToLower();
            RootNamespace = MsBuildProject.GetProperty("RootNamespace") ?? AssemblyName;
            SourceFiles = MsBuildProject.Items["Compile"].Select(i => i.ItemSpec).ToList();
            StartupObject = MsBuildProject.GetProperty("StartupObject");
            TargetPath = MsBuildProject.GetProperty("TargetPath");
            TargetDir = MsBuildProject.GetProperty("TargetDir");
            TargetExt = MsBuildProject.GetProperty("TargetExt");
            TargetFramework = MsBuildProject.TargetFramework;
            NoStdLib = MsBuildProject.GetProperty("NoStdLib").ToLower() == "true" ? true : false;
            PackageReferences =
                MsBuildProject.PackageReferences
                .Select(r => new AssemblyName(r.Key) { Version = r.Value.ContainsKey("Version") ? r.Value["Version"].ToVersion() : null })
                .Select(n => new AssemblyReference(n, Metadata.Assembly.TryResolve(n, ProjectFile.DirectoryName!)))
                .ToList();
            References = MsBuildProject.References.ToList();
            op.Complete();
        }
        Initialized = true;     
    }
    #endregion

    #region Properties
    public IAnalyzerResult? MsBuildProject { get; init;}
    
    public string? TargetDir { get; set; }
    
    public string? TargetExt { get; init; }

    public string? TargetFramework { get; init; }

    public string? BuildConfiguration { get; init; } 

    public List<AssemblyReference>? PackageReferences { get; init; }
    #endregion
}

