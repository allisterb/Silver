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
