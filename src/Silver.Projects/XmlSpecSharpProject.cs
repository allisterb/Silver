using System.Xml.Serialization;
using System.Xml;

namespace Silver.Projects
{
    public class XmlSpecSharpProject : SpecSharpProject
    {
        #region Constructor
        public XmlSpecSharpProject(string filePath, string buildConfig) : base(filePath, buildConfig)
        {
            using (var op = Begin("Loading XML Spec# project {0}", filePath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Models.VisualStudioProject));
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    Model = (Models.VisualStudioProject?) ser.Deserialize(reader);
                    if (Model is null)
                    {
                        Error("This file is not a valid Spec# XML project file.");
                        op.Cancel();
                        return;
                    }
                    AssemblyName = Model.XEN.Build.Settings.AssemblyName;
                    OutputType = Model.XEN.Build.Settings.OutputType.ToLower();
                    RootNamespace = Model.XEN.Build.Settings.RootNamespace;
                    StartupObject = Model.XEN.Build.Settings.StartupObject;
                    StandardLibraryLocation = Model.XEN.Build.Settings.StandardLibraryLocation;
                    TargetPlatformLocation = Model.XEN.Build.Settings.TargetPlatformLocation;
                    ShadowedAssembly = Model.XEN.Build.Settings.ShadowedAssembly;
                    if (Model.XEN.Build.References is not null && Model.XEN.Build.References.Any())
                    {
                        References = new();
                        foreach (var r in Model.XEN.Build.References)
                        {
                            if (!string.IsNullOrEmpty(r.Project) && Guid.TryParse(r.Project, out var _))
                            {
                                Error("Project reference using a GUID is not supported. Use a project file reference for {0} with GUID {1}.", r.Name, r.Project);
                            }
                            else if (!string.IsNullOrEmpty(r.AssemblyName) && !string.IsNullOrEmpty(r.HintPath))
                            {
                                References.Add(Path.Combine(ProjectFile.DirectoryName!, r.HintPath.Replace("/", PathSeparator)));
                            }
                            else if (!string.IsNullOrEmpty(r.AssemblyName))
                            {
                                References.Add(r.AssemblyName + ".dll");
                            }
                        }
                    }
                    foreach (var f in Model.XEN.Files.Include)
                    {
                        if (f.BuildAction == "Compile")
                        {
                            SourceFiles.Add(Path.Combine(ProjectFile.Directory!.FullName, f.RelPath.Replace("/", PathSeparator)));
                        }
                    }
                    if (Model.XEN.Build.Settings.Config.Any(c => c.Name == RequestedBuildConfig))
                    {
                        BuildConfiguration = RequestedBuildConfig;
                        var config = Model.XEN.Build.Settings.Config.First(c => c.Name == RequestedBuildConfig);
                        TargetPath = config.OutputPath;
                        DefineConstants = config.DefineConstants;
                        op.Complete();
                        Initialized = true;
                    }
                    else
                    {
                        Error("The requested build configuration {0} does not exist in the project file.", RequestedBuildConfig);
                        op.Cancel();
                    }
                }
            }
        }
        #endregion

        #region Properties
        protected Models.VisualStudioProject? Model { get; init; }
        #endregion
    }
}
