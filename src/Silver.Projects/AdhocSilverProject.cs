namespace Silver.Projects;

using Microsoft.CodeAnalysis;
public class AdhocSilverProject : SilverProject
{
    #region Constructirs
    public AdhocSilverProject(Dictionary<string, object> settings) : base(settings.FailIfKeyNotPresent<IEnumerable<string>> ("SourceFiles").First(), settings.FailIfKeyNotPresent<string>("BuildConfig"))
    {
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
        ProjectInfo project = 
            ProjectInfo
            .Create(projectId, versionStamp, projectName, projectName, LanguageNames.CSharp, documents: docs);
        RoslynWorkspace.AddProject(project);
        Initialized = true;
    }
    #endregion

    #region Overriden members
    public override bool NativeBuild() => this.SscCompile().Succeded;
    #endregion
}

