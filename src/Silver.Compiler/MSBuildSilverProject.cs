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
            Debug("Target .NET frameworks are {0}.", _results.Where(r => r.Succeeded && !string.IsNullOrEmpty(r.TargetFramework)).Select(r => r.TargetFramework));
            if (_results.Count(r => r.Succeeded) == 1)
            {
                MsBuildProject = _results.First(r => r.Succeeded);
            }
            else
            {
                if (_results.Any(r => r.Succeeded && r.TargetFramework == "net461"))
                {
                    MsBuildProject = _results.First(r => r.Succeeded && r.TargetFramework == "net461");
                }
                else
                {
                    MsBuildProject = _results.First(r => r.Succeeded && r.Items.ContainsKey("Compile"));
                }
            }
            if(!MsBuildProject.Items.ContainsKey("Compile"))
            {
                Error("Cannot determine which files to compile in MSBuild project.");
                return;
            }
            TargetFramework = MsBuildProject.TargetFramework;
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
            NoStdLib = MsBuildProject.Properties.ContainsKey("NoStdLib") && MsBuildProject.GetProperty("NoStdLib").ToLower() == "true" ? true : false;
            PackageReferences =
                MsBuildProject.PackageReferences
                .Select(r => new AssemblyName(r.Key) { Version = r.Value.ContainsKey("Version") ? r.Value["Version"].ToVersion() : null })
                .Select(n => new AssemblyReference(n, Metadata.Assembly.TryResolve(n, ProjectFile.DirectoryName!)))
                .ToList();
            //References = MsBuildProject.References.Where(r => !r.Contains("Microsoft.NETCore.App.Ref")).ToList();
            //if ((MsBuildProject.Items.ContainsKey("Reference")) && MsBuildProject.Items["Reference"].Any(r => r.Metadata["IsImplicitlyDefined"] == "true"))
            //{
            //    GACReferences
            //        .AddRange(MsBuildProject.Items["Reference"]
            //        .Where(r => r.Metadata["IsImplicitlyDefined"] == "true")
            //        .Select(r => new AssemblyGACReference(r.ItemSpec, true))); //new Asssr.ItemSpec);
            //}
            References.AddRange(GACReferences.Select(r => r.Name + ".dll"));
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

}

