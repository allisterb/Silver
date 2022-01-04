namespace Silver.SpecSharp;

using Microsoft.Build;
using Microsoft.Build.Evaluation;
public class CsSpecSharpProject : SpecSharpProject
{
    #region Constructors
    public CsSpecSharpProject(string filePath) : base(filePath)
    {
        var collection = new ProjectCollection();
        MsBuildProject = collection.LoadProject(filePath);
        if (MsBuildProject is not null)
        {
            AssemblyName = MsBuildProject.AllEvaluatedProperties.Single(p => p.Name == "AssemblyName").EvaluatedValue;
            OutputType = MsBuildProject.AllEvaluatedProperties.Single(p => p.Name == "OutputType").EvaluatedValue;
            RootNamespace = MsBuildProject.AllEvaluatedProperties.Single(p => p.Name == "RootNamespace").EvaluatedValue;
            Initialized = true;
        }
    }
    #endregion

    #region Properties
    public Project? MsBuildProject { get; init;}
    
    #endregion
}

