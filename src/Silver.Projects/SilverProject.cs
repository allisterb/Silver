namespace Silver.Projects;

#region Records
internal readonly record struct AssemblyFileReference(string Name, string HintPath, bool isprivate);

internal readonly record struct AssemblyProjectReference(string Name, SilverProject Project, bool isprivate);

internal readonly record struct AssemblyGACReference(string Name, bool isprivate);
#endregion

public abstract class SilverProject : Runtime
{
    #region Constructors
    public SilverProject(string filePath, string buildConfig, SilverProject? parent = null) :base() 
    {
        ProjectFile = new FileInfo(FailIfFileNotFound(filePath));
        Parent = parent;
        Debug("Project directory is {0}.", ProjectFile.DirectoryName!);
        RequestedBuildConfig = buildConfig;
        TargetPlatform = "v4";
    }
    #endregion

    #region Properties
    public FileInfo ProjectFile { get; init; }

    protected SilverProject? Parent { get; init; }

    public string RequestedBuildConfig { get; init; }

    public string AssemblyName { get; protected set; } = string.Empty;

    public string DefineConstants { get; protected set; } = string.Empty;

    public string OutputType { get; protected set; } = string.Empty;

    public bool DebugEnabled 
    {
        get => RequestedBuildConfig.StartsWith("Debug") || RequestedBuildConfig.EndsWith("Debug"); 
    } 

    public string? RootNamespace { get; protected set; } = string.Empty;

    public List<string> SourceFiles { get; init; } = new();

    public string TargetPath { get; protected set; } = string.Empty;

    public string? TargetDir { get; protected set; }

    public string? TargetExt { get; init; }

    public string StartupObject { get; protected set; } = string.Empty;

    public string StandardLibraryLocation { get; protected set; } = string.Empty;

    public string TargetPlatform { get; protected set; } = string.Empty;

    public string TargetPlatformLocation { get; protected set; } = string.Empty;

    public string ShadowedAssembly { get; protected set; } = string.Empty;

    public bool AllowUnsafe { get; protected set; } = false;

    public List<string> References { get; protected set; } = new();

    internal List<AssemblyFileReference> FileReferences { get; } = new();

    internal List<AssemblyProjectReference> ProjectReferences { get; } = new();

    internal List<AssemblyGACReference> GACReferences { get; } = new();

    public bool NoStdLib { get; protected set; } = false;

    public string? BuildConfiguration { get; init; }

    public bool BuildUpToDate { get; protected set; }

    public bool Verify { get; set; } = false;

    
    public string CommandLine
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/platform:v4 ");
            if (!string.IsNullOrEmpty(TargetPath))
            {
                sb.AppendFormat("/out:{0} ", TargetPath);
            }
            if (!string.IsNullOrEmpty(OutputType))
            {
                sb.AppendFormat("/target:{0} ", OutputType);
            }
            if (!string.IsNullOrEmpty(DefineConstants))
            {
                sb.AppendFormat("/define:{0} ", DefineConstants);
            }
            if (DebugEnabled)
            {
                sb.Append("/debug+ /debug:pdbonly ");
            }
            if (NoStdLib)
            {
                sb.Append("/nostdlib+ ");
            }
            if (!string.IsNullOrEmpty(ShadowedAssembly))
            {
                sb.AppendFormat("/shadow:{0} ", Path.Combine(ProjectFile.DirectoryName!, ShadowedAssembly));
            }
            if (AllowUnsafe)
            {
                sb.Append("/unsafe+ ");
            }
            if (Verify)
            {
                sb.Append("/verify ");
            }
            if (References.Any())
            {
                sb.AppendFormat("/r:{0} ", References.JoinWith(";"));
            }
            sb.Append(SourceFiles.JoinWithSpaces());
            return sb.ToString().TrimEnd();
        }
    }

    public IEnumerable<string> PublicReferences
    {
        get => References.Where(r => 
            !FileReferences.Any(fr => fr.HintPath == r && fr.isprivate) && 
            !ProjectReferences.Any(pr => pr.Project.TargetPath == r && pr.isprivate) &&
            !GACReferences.Any(gr => gr.Name == r && gr.isprivate)
        );
    }
    #endregion

    #region Abstract methods
    public abstract bool NativeBuild();
    #endregion

    #region Methods
    public SilverProjectCompilation Compile()
    {
        FailIfNotInitialized();
        using (var op = Parent is null ?  
            Begin("Compiling Spec# project using configuration {0}", BuildConfiguration!) : Begin("Compiling Spec# reference for project {0} using configuration {1}", Parent.ProjectFile.Name, BuildConfiguration!))
        {
            var compilerErrors = new List<CompilerError>();
            var compilerWarnings = new List<CompilerWarning>();

            var output = RunCmd(Path.Combine(AssemblyLocation, "ssc", "ssc.exe"), CommandLine, Path.Combine(AssemblyLocation, "ssc"),
                (sender, e) => 
                {
                    if (e.Data is not null && e.Data.Contains("error CS") && !e.Data.Trim().StartsWith("error"))
                    {
                        var errs = e.Data.Split(": error");
                        var errmsg = errs[1].Split(":");
                        compilerErrors.Add(new(errs[0], errmsg[0], errmsg[1]));
                        Error("File: " + errs[0] + Environment.NewLine + "               Code:{0}" + Environment.NewLine + 
                            "               Msg: {1}", errmsg[0], errmsg[1]); 
                    }
                    else if (e.Data is not null && e.Data.Contains("error CS") && e.Data.Trim().StartsWith("error"))
                    {
                        var err = e.Data.Split("error ").Last().Split(":");
                        Error("Code:{0}" + Environment.NewLine + "               Msg:{1}", err[0], err.Skip(1).JoinWith(""));
                    }
                    else if (e.Data is not null && e.Data.Contains("error CS") && e.Data.Trim().StartsWith("fatal error"))
                    {
                        var err = e.Data.Split("fatal error ").Last().Split(":");
                        Error("Code:{0}" + Environment.NewLine + "               Msg:{1}", err[0], err.Skip(1).JoinWith(""));
                    }
                    else if (e.Data is not null && e.Data.Contains("error"))
                    {
                        var errs = e.Data.Split("error:");
                        Error(errs.Last());
                    }
                    else if (e.Data is not null && e.Data.Contains("warning CS") && !e.Data.Trim().StartsWith("warning"))
                    {
                        var warns = e.Data.Split(": warning");
                        var warnmsg = warns[1].Split(":");
                        compilerWarnings.Add(new(warns[0], warnmsg[0], warnmsg[1]));
                        Warn("File: " + warns[0] + Environment.NewLine + "               Code:{0}" + Environment.NewLine +
                            "               Msg: {1}", warnmsg[0], warnmsg[1]);
                    }
                });

            if (output is null || output.Contains("error"))
            {
                Error("Compile failed.");
                op.Cancel();
                return new SilverProjectCompilation(this, false, Verify, compilerErrors, compilerWarnings);
            }
            else
            {
                if (!Verify)
                {
                    foreach (var r in PublicReferences)
                    {
                        var cr = Path.Combine(Path.GetDirectoryName(TargetPath)!, Path.GetFileName(r));
                        if (File.Exists(r) && (!File.Exists(cr) || (File.GetLastWriteTime(r) > File.GetLastWriteTime(cr))))
                        {
                            if (File.Exists(cr)) File.Delete(cr);
                            File.Copy(r, cr);
                            var pr = Path.ChangeExtension(r, ".pdb");
                            if (File.Exists(pr))
                            {
                                var pcr = Path.Combine(Path.GetDirectoryName(TargetPath)!, Path.GetFileName(pr));
                                if (File.Exists(pcr)) File.Delete(pcr);
                                File.Copy(pr, pcr);
                            }
                            Info("Copied reference {0}.", Path.GetFileName(r));
                        }
                        else if (File.Exists(r))
                        {
                            Debug("Not copying reference {0} as it already exists.", r);
                        }
                    }
                    op.Complete();
                    Info("Compile succeded. Assembly is at {0}.", TargetPath);
                }
                else 
                {
                    File.Delete(TargetPath);
                    op.Complete();
                    var vwarn = compilerWarnings.Where(w => w.Msg.ToLower().Contains("unsatisfied"));
                    if (vwarn.Count() > 0)
                    {
                        Info("Verification completed with {0} warnings. Target assembly not retained.", vwarn.Count());
                    }
                    else
                    {
                        Info("Verification succeded. Target assembly not retained");
                    }
                }
                return new SilverProjectCompilation(this, true, Verify, compilerErrors, compilerWarnings);
            }
        }
    }

    public static bool HasProjectExtension(string f)
    {
        switch (Path.GetExtension(f))
        {
            case ".csproj":
            case ".sscproj":
            case ".cs":
            case ".ssc":
                return true;
            default: return false;
        }
    }
    public static SilverProject? GetProject(string filePath, string buildConfig, params string [] additionalFiles)
    {  
        var f = new FileInfo(FailIfFileNotFound(filePath));
        switch(f.Extension)
        {
            case ".csproj":
                return new MSBuildSilverProject(f.FullName, buildConfig);
            case ".sscproj":
                return new XmlSilverProject(f.FullName, buildConfig);
            case ".cs":
            case ".ssc":
                var sourceFiles = additionalFiles.ToList().Prepend(filePath).ToList();
                var settings = new Dictionary<string, object> 
                {
                    { "BuildConfig", "Debug" },
                    { "SourceFiles", sourceFiles } 
                };
                return new AdhocSilverProject(settings);
            default:
                Error("The file {0} has an unrecognized extension. Valid extensions for Spec# projects are {1}, {2}, {3}, and {4}.", f.FullName, ".csproj", ".sscproj", ".cs", ".ssc");
                return null;
        }
    }

    public static object? GetProperty(string filePath, string buildConfig, string prop, params string[] additionalFiles)
    {
        var file = new FileInfo(FailIfFileNotFound(filePath));
        if (file.Extension == ".csproj")
        {
            var proj = new MSBuildSilverProject(filePath, buildConfig);
            return proj.Initialized ? GetProp(proj, prop) : null;
        }
        else if (file.Extension == ".sscproj")
        {
            var proj = new XmlSilverProject(filePath, buildConfig);
            return proj.Initialized ? GetProp(proj, prop) : null;
        }
        else if (file.Extension == ".ssc")
        {
            var sourceFiles = additionalFiles.Prepend(filePath).ToList();
            var settings = new Dictionary<string, object>
                {
                    { "BuildConfig", "Debug" },
                    { "SourceFiles", sourceFiles }
                };
            var proj = new AdhocSilverProject(settings);
            return proj.Initialized ? GetProp(proj, prop) : null;
        }
        else
        {
            Error("The file {0} has an unrecognized extension. Valid extensions for Spec# projects are {1} and {2}.", filePath, ".csproj", ".sscproj");
            return null;
        }
    }
    #endregion
}