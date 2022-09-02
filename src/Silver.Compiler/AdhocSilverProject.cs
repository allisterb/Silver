namespace Silver.Compiler;

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
       
        string projectName = "SmartContract";
        ProjectId projectId = ProjectId.CreateNewId();
        VersionStamp versionStamp = VersionStamp.Create();
        var docs = SourceFiles.Select(f => DocumentInfo.Create(DocumentId.CreateNewId(projectId), Path.GetFileName(f), sourceCodeKind: SourceCodeKind.Regular, filePath: f));
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
            .Create(projectId, versionStamp, projectName, projectName, LanguageNames.CSharp, documents: docs)
            .WithCompilationOptions(options);
            //.WithMetadataReferences(SctAssemblies.Select(a => MetadataReference.CreateFromFile(a.Location)));
        RoslynWorkspace.AddProject(project);
        op.Complete();
        Initialized = true;
    }
    #endregion

    #region Properties
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