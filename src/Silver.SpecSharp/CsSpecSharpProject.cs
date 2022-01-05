namespace Silver.SpecSharp;

using Microsoft.Build;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
public class CsSpecSharpProject : SpecSharpProject
{
    #region Constructors
    static CsSpecSharpProject()
    {
       
    }
    public CsSpecSharpProject(string filePath) : base(filePath)
    {
        var vs = MSBuildLocator.QueryVisualStudioInstances();
        var collection = new ProjectCollection();
        MsBuildProject = collection.LoadProject(filePath);
        if (MsBuildProject is not null)
        {
            AssemblyName = MsBuildProject.AllEvaluatedProperties.SingleOrDefault(p => p.Name == "AssemblyName")?.EvaluatedValue ?? ProjectFile.Name.Replace(ProjectFile.Extension, "");
            OutputType = MsBuildProject.AllEvaluatedProperties.Single(p => p.Name == "OutputType")?.EvaluatedValue ?? "library";
            RootNamespace = MsBuildProject.AllEvaluatedProperties.Single(p => p.Name == "RootNamespace")?.EvaluatedValue ;
            TargetPlatform = "v4";

            Initialized = true;
        }
    }
    #endregion

    #region Properties
    public Project? MsBuildProject { get; init;}
    
    #endregion
}

