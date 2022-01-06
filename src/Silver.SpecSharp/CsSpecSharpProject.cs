namespace Silver.SpecSharp;

using System.Reflection;

using Microsoft.Build;
using Microsoft.Build.Evaluation;
using Nuclear.Assemblies.ResolverData;

using Silver.Metadata;

public class CsSpecSharpProject : SpecSharpProject
{
    #region Constructors
    static CsSpecSharpProject()
    {
       
    }
    public CsSpecSharpProject(string filePath) : base(filePath)
    {
        var collection = new ProjectCollection();
        MsBuildProject = collection.LoadProject(filePath);
        if (MsBuildProject is not null)
        {
            AssemblyName = MsBuildProject.AllEvaluatedProperties.SingleOrDefault(p => p.Name == "AssemblyName")?.EvaluatedValue ?? ProjectFile.Name.Replace(ProjectFile.Extension, "");
            OutputType = MsBuildProject.AllEvaluatedProperties.FirstOrDefault(p => p.Name == "OutputType")?.EvaluatedValue.ToLower() ?? "library";
            RootNamespace = MsBuildProject.AllEvaluatedProperties.SingleOrDefault(p => p.Name == "RootNamespace")?.EvaluatedValue ;
            TargetPlatform = "v4";
            TargetExt = MsBuildProject.AllEvaluatedProperties.Single(p => p.Name == "TargetExt").EvaluatedValue;
            TargetDir = MsBuildProject.AllEvaluatedProperties.Single(p => p.Name == "TargetDir").EvaluatedValue;
            TargetPath = MsBuildProject.AllEvaluatedProperties.Single(p => p.Name == "TargetPath").EvaluatedValue;
            StartupObject = MsBuildProject.AllEvaluatedProperties.SingleOrDefault(p => p.Name == "StartupObject")?.EvaluatedValue;
            BuildConfiguration = MsBuildProject.AllEvaluatedProperties.Single(p => p.Name == "Configuration").EvaluatedValue;
            
            PackageReferences =
                MsBuildProject.AllEvaluatedItems
                .Where(i => i.ItemType == "PackageReference")
                .Select(i => new AssemblyName(i.EvaluatedInclude) {Version = i.Metadata.FirstOrDefault(m => m.Name == "Version")?.EvaluatedValue.ToVersion() })
                .Select(r => new AssemblyReference(r, Metadata.Assembly.TryResolve(r, ProjectFile.DirectoryName!)))
                .ToList();

            Initialized = true;
        }
    }
    #endregion

    #region Properties
    public Project? MsBuildProject { get; init;}

    public List<AssemblyReference> PackageReferences { get; init;}  
    public string? TargetDir { get; set; }
    public string? TargetExt { get; init; }
    public List<Project>? ProjectReferences { get; }
    
    public string BuildConfiguration { get; }
    #endregion
}

