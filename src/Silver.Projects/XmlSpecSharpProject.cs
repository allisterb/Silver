using System.Xml.Serialization;
using System.Xml;

namespace Silver.Projects
{
    public class XmlSpecSharpProject : SpecSharpProject
    {
        #region Constructor
        public XmlSpecSharpProject(string fileName, string buildConfig) : base(fileName, buildConfig)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Models.XmlSpecSharpProjectModel));
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                Model = (Models.XmlSpecSharpProjectModel?) ser.Deserialize(reader);
                if (Model is not null)
                {
                    AssemblyName = Model.XEN.Build.Settings.AssemblyName;
                    //DebugEnabled = Model.XEN.Build.Settings.
                    OutputType = Model.XEN.Build.Settings.OutputType;
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
                                Error("Project reference using a GUID is not supported. Use a project file reference for {0} with GUID {1}", r.Name, r.Project);
                                return;
                            }
                            else if (!string.IsNullOrEmpty(r.AssemblyName) && !string.IsNullOrEmpty(r.HintPath))
                            {
                                References.Add(Path.Combine(ProjectFile.DirectoryName!, r.HintPath));
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
                            SourceFiles.Add(Path.Combine(ProjectFile.Directory!.FullName, f.RelPath));
                        }
                    }
                }    
                
            }
        }
        #endregion

        #region Properties
        protected Models.XmlSpecSharpProjectModel? Model { get; init; }
        #endregion
    }
}
