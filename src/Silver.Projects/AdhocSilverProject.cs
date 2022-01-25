namespace Silver.Projects;

using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Buildalyzer;
using Buildalyzer.Environment;
using Buildalyzer.Workspaces;
public class AdhocSilverProject : SilverProject
{
    #region Constructors
    public AdhocSilverProject(Dictionary<string, object> settings) : base(settings.FailIfKeyNotPresent<IEnumerable<string>> ("SourceFiles").First(), settings.FailIfKeyNotPresent<string>("BuildConfig"))
    {
        using var op = Begin("Loading ad hoc project");
        BuildConfiguration = settings.TryGet<string>("BuildConfiguration") ?? "Debug";
        SourceFiles = (List<string>) settings["SourceFiles"];
        AssemblyName = settings.TryGet<string>("AssemblyName") ?? Path.GetFileNameWithoutExtension(SourceFiles.First());
        OutputType = settings.TryGet<string>("OutputType") ?? "library";
        TargetExt = OutputType.ToLower() == "exe" ? ".exe" : ".dll";
        TargetPath = settings.TryGet<string>("TargetPath") ?? Path.Combine(Path.GetDirectoryName(SourceFiles.First())!, AssemblyName) + TargetExt;
        RootNamespace = settings.TryGet<string>("RootNamespace");
        StartupObject = settings.TryGet<string>("StartupObject") ?? "library";
        StandardLibraryLocation = settings.TryGet<string>("StandardLibraryLocation") ?? "";
        TargetPlatformLocation = settings.TryGet<string>("TargetPlatformLocation") ?? "";
        ShadowedAssembly = settings.TryGet<string>("ShadowedAssembly") ?? "";
        StringWriter log = new StringWriter();
        AnalyzerManagerOptions ao = new AnalyzerManagerOptions
        {
            LogWriter = log
        };
        var m = new AnalyzerManager(ao);
        m.SetGlobalProperty("Configuration", RequestedBuildConfig);
        var a = m.GetProject(Path.Combine(AssemblyLocation, "BlockchainContractProj.xml"));
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
        var proj = _results.First(r => r.Succeeded).AddToWorkspace(RoslynWorkspace);
        var docs = SourceFiles.Select(f => DocumentInfo.Create(DocumentId.CreateNewId(proj.Id), Path.GetFileNameWithoutExtension(f), null, SourceCodeKind.Regular, null, f));
        foreach (var doc in docs) RoslynWorkspace.AddDocument(doc); 
        op.Complete();
        /*
        string projectName = "SmartContract";
        ProjectId projectId = ProjectId.CreateNewId();
        VersionStamp versionStamp = VersionStamp.Create();
        //var docs = SourceFiles.Select(f => DocumentInfo.Create(DocumentId.CreateNewId(projectId), Path.GetFileName(f), sourceCodeKind: SourceCodeKind.Regular, filePath: f));
        var options = 
            new CSharpCompilationOptions
            (
                OutputKind.DynamicallyLinkedLibrary,
                checkOverflow: true,
                optimizationLevel: OptimizationLevel.Debug,
                deterministic: true
            );
        ProjectInfo project = 
            ProjectInfo
            .Create(projectId, versionStamp, projectName, projectName, LanguageNames.CSharp, "BlockchainContractProj.xml", documents: docs)
            .WithCompilationOptions(options)
            .WithMetadataReferences(SctAssemblies.Select(a => MetadataReference.CreateFromFile(a.Location)));
        RoslynWorkspace.AddProject(project);
        */
        Initialized = true;
    }
    #endregion

    #region Overriden members
    public override bool NativeBuild() => this.SscCompile().Succeded;
    #endregion

    #region Properties
    public static string Proj =
        "<Project Sdk = \"Microsoft.NET.Sdk\" ><PropertyGroup><TargetFramework> netcoreapp3.1</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include = \"Stratis.SmartContracts\" Version=\"2.0.0\" /></ItemGroup></Project>";
    public static HashSet<Assembly> SctAssemblies =
       new HashSet<Assembly>
       {
            typeof(object).Assembly,
            Assembly.Load("System.Runtime"),
            typeof(Stratis.SmartContracts.SmartContract).Assembly,
            typeof(Enumerable).Assembly,
            typeof(Stratis.SmartContracts.Standards.IStandardToken).Assembly
       };
    #endregion
}