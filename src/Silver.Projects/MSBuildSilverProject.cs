namespace Silver.Projects;

using System.Reflection;
using Roslyn = Microsoft.CodeAnalysis;

using Buildalyzer;
using Buildalyzer.Environment;
using Buildalyzer.Workspaces;
using Silver.Metadata;

public class MSBuildSilverProject : SilverProject
{
    #region Constructors
    static MSBuildSilverProject() { }
    public MSBuildSilverProject(string filePath, string buildConfig) : base(filePath, buildConfig)
    {
        using (var op = Begin("Loading MSBuild C# project {0}", ViewFilePath(filePath)))
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
            var envOptions = new EnvironmentOptions() { };
            envOptions.TargetsToBuild.Remove("Clean");
            var _results = a.Build(envOptions);
            foreach (var l in log.ToString().Split(Environment.NewLine))
            {
                Debug(l);
            }
            if (!_results.Any(r => r.Succeeded))
            {
                Fatal("Design-time build of {0} failed.", ProjectFile.FullName);
                op.Cancel();
                return;
            }
            MsBuildProject = _results.First(r => r.Succeeded);
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
            BuildUpToDate =
                !string.IsNullOrEmpty(TargetPath) &&
                File.Exists(TargetPath) &&
                SourceFiles.All(f => File.GetLastWriteTime(f) <= File.GetLastWriteTime(TargetPath)) &&
                ProjectFile.LastWriteTime <= File.GetLastWriteTime(TargetPath);
            MsBuildProject.AddToWorkspace(RoslynWorkspace);
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

    #region Overriden members
    public override bool NativeBuild()
    {
        var r = RunCmd("dotnet", "build", this.ProjectFile.DirectoryName, checkExists: false);
        return !string.IsNullOrWhiteSpace(r);
    }
    #endregion
}

