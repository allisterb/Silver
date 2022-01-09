using System.Xml.Serialization;
using System.Xml;

namespace Silver.Projects
{
    public class XmlSpecSharpProject : SpecSharpProject
    {
        #region Constructor
        public XmlSpecSharpProject(string filePath, string buildConfig, XmlSpecSharpProject? parent = null) : base(filePath, buildConfig, parent)
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
                                continue;
                            }
                            else if (!string.IsNullOrEmpty(r.Project))
                            {
                                var prf = Path.Combine(ProjectFile.DirectoryName!, r.Project.NormalizeFilePath());
                                if (!File.Exists(prf))
                                {
                                    Error("The project reference {0} does not exist.", prf);
                                    continue;
                                }
                                var pr = new XmlSpecSharpProject(prf, buildConfig, this);
                                if (!pr.Initialized)
                                {
                                    Error("The project reference {0} could not be loaded.", pr.ProjectFile.FullName);
                                    continue;
                                }
                                else if(!pr.Compile())
                                {
                                    Error("The project reference {0} could not be built.", pr.ProjectFile.FullName);
                                    continue;
                                }
                                else
                                {
                                    References.Add(pr.TargetPath);
                                    Debug("Added project reference {0}.", pr.TargetPath);
                                    if (!r.Private && pr.PublicReferences.Any())
                                    {
                                        Debug("Added transitive references {0} for project {1}.", pr.PublicReferences, pr.TargetPath);
                                        References.AddRange(pr.PublicReferences);
                                    }
                                    ProjectReferences.Add(new(r.Project, pr, r.Private));
                                }
                            }
                            else if (!string.IsNullOrEmpty(r.AssemblyName) && !string.IsNullOrEmpty(r.HintPath))
                            {
                                var hp = Path.Combine(ProjectFile.DirectoryName!, r.HintPath.NormalizeFilePath());
                                References.Add(hp);
                                FileReferences.Add(new(r.AssemblyName, hp, r.Private));
                            }
                            else if (!string.IsNullOrEmpty(r.AssemblyName))
                            {
                                GACReferences.Add(new(r.AssemblyName + ".dll", r.Private));
                                References.Add(r.AssemblyName + ".dll");
                            }
                        }
                    }
                    foreach (var f in Model.XEN.Files.Include)
                    {
                        if (f.BuildAction == "Compile")
                        {
                            SourceFiles.Add(Path.Combine(ProjectFile.Directory!.FullName, f.RelPath.NormalizeFilePath()));
                        }
                    }
                    if (Model.XEN.Build.Settings.Config.Any(c => c.Name == RequestedBuildConfig))
                    {
                        BuildConfiguration = RequestedBuildConfig;
                        var config = Model.XEN.Build.Settings.Config.First(c => c.Name == RequestedBuildConfig);
                        TargetDir = Path.Combine(ProjectFile.DirectoryName!, config.OutputPath.NormalizeFilePath())!;
                        if (!Directory.Exists(TargetDir))
                        {
                            Debug("Creating target directory {0}.", TargetDir);
                            Directory.CreateDirectory(TargetDir);
                        }
                        TargetExt = OutputType.ToLower() == "exe" ? ".exe" : ".dll";
                        TargetPath = Path.Combine(TargetDir, AssemblyName + TargetExt);
                        DefineConstants = config.DefineConstants;
                        AllowUnsafe = config.AllowUnsafeBlocks.ToLower() == "true" ? true : false;
                        Initialized = true;
                        op.Complete();
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
