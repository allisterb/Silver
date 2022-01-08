namespace Silver.Projects;

using System.Reflection;

using Buildalyzer;

using Silver.Metadata;

public class MSBuildSpecSharpProject : SpecSharpProject
{
    #region Constructors
    static MSBuildSpecSharpProject() { }
    public MSBuildSpecSharpProject(string filePath, string buildConfig) : base(filePath, buildConfig)
    {
        using (var op = Begin("Loading MSBuild Spec# project {0}", filePath))
        {
            StringWriter log = new StringWriter();
            AnalyzerManagerOptions options = new AnalyzerManagerOptions
            {
                LogWriter = log
            };
            var m = new AnalyzerManager(options);
            m.SetGlobalProperty("Configuration", RequestedBuildConfig);
            var a = m.GetProject(filePath);
            Debug("Performing a design-time build of {0} in configuration {1}.", ProjectFile.FullName, RequestedBuildConfig);
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
            SourceFiles = MsBuildProject.Items["Compile"].Select(i => Path.Combine(ProjectFile.DirectoryName!, i.ItemSpec)).ToList();
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
            References = MsBuildProject.References.Where(r => !r.Contains("Microsoft.NETCore.App.Ref")).ToList();
            Initialized = true;
            op.Complete();
        }
             
    }
    #endregion

    #region Properties
    public IAnalyzerResult? MsBuildProject { get; init;}
     
    public string? TargetFramework { get; init; }

    public List<AssemblyReference>? PackageReferences { get; init; }
    #endregion
}

