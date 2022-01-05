using System.Xml.Serialization;
using System.Xml;

namespace Silver.SpecSharp
{
    public class XmlSpecSharpProject : SpecSharpProject
    {
        public XmlSpecSharpProject(string fileName) : base(fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(VisualStudioProject));
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                Model = (VisualStudioProject?) ser.Deserialize(reader);
                if (Model is not null)
                {
                    AssemblyName = Model.XEN.Build.Settings.AssemblyName;
                    OutputType = Model.XEN.Build.Settings.OutputType;
                    RootNamespace = Model.XEN.Build.Settings.RootNamespace;
                    StartupObject = Model.XEN.Build.Settings.StartupObject;
                    StandardLibraryLocation = Model.XEN.Build.Settings.StandardLibraryLocation;
                    TargetPlatform = "v4";
                    TargetPlatformLocation = Model.XEN.Build.Settings.TargetPlatformLocation;
                    ShadowedAssembly = Model.XEN.Build.Settings.ShadowedAssembly;
                    if (Model.XEN.Build.References is not null && Model.XEN.Build.References.Any())
                    {
                        foreach (var r in Model.XEN.Build.References)
                        {
                            if (!string.IsNullOrEmpty(r.Project) && Guid.TryParse(r.Project, out var _))
                            {
                                Error("Project reference using a GUID is not supported. Use a project file reference for {0} with GUID {1}", r.Name, r.Project);
                                return;
                            }
                        }
                    }
                    foreach (var f in Model.XEN.Files.Include)
                    {
                        if (f.BuildAction == "Compile")
                        {
                            FilesToCompile.Add(Path.Combine(ProjectFile.Directory!.FullName, f.RelPath));
                        }
                    }
                }    
                
            }
        }

        protected VisualStudioProject? Model { get; init; }
    }

    
}
