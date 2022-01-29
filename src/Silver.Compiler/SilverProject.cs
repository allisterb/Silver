namespace Silver.Projects;

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;

using SharpSyntaxRewriter.Rewriters;
using SharpSyntaxRewriter.Rewriters.Types;
using Silver.CodeAnalysis.Cs;
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

    public AdhocWorkspace RoslynWorkspace { get; } = new AdhocWorkspace();

    public string RequestedBuildConfig { get; init; }

    public string AssemblyName { get; protected set; } = string.Empty;

    public string DefineConstants { get; protected set; } = string.Empty;

    public string OutputType { get; protected set; } = string.Empty;

    public bool DebugConfig
    {
        get => RequestedBuildConfig.StartsWith("Debug") || RequestedBuildConfig.EndsWith("Debug"); 
    } 

    public string? RootNamespace { get; protected set; } = string.Empty;

    public List<string> SourceFiles { get; protected set; } = new();

    public List<string>? OriginalSourceFiles { get; protected set; } 

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
            sb.Append("-platform:v4 ");
            if (!string.IsNullOrEmpty(TargetPath))
            {
                sb.AppendFormat("-out:{0} ", TargetPath);
            }
            if (!string.IsNullOrEmpty(OutputType))
            {
                sb.AppendFormat("-target:{0} ", OutputType);
            }
            if (!string.IsNullOrEmpty(DefineConstants))
            {
                sb.AppendFormat("-define:{0} ", DefineConstants);
            }
            if (DebugConfig)
            {
                sb.Append("-debug+ -debug:pdbonly ");
            }
            if (NoStdLib)
            {
                sb.Append("-nostdlib+ ");
            }
            if (!string.IsNullOrEmpty(ShadowedAssembly))
            {
                sb.AppendFormat("-shadow:{0} ", Path.Combine(ProjectFile.DirectoryName!, ShadowedAssembly));
            }
            if (AllowUnsafe)
            {
                sb.Append("-unsafe+ ");
            }
            if (Verify)
            {
                sb.Append("-verify ");
            }
            if (References.Any())
            {
                sb.AppendFormat("-r:{0} ", References.Select(f => Path.GetRelativePath(Path.Combine(AssemblyLocation, "ssc"), f)).JoinWith(";"));
            }
            sb.Append(SourceFiles.Select(f => Path.GetRelativePath(Path.Combine(AssemblyLocation, "ssc"), f)).JoinWithSpaces());
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
    public virtual bool Compile(out IEnumerable<Diagnostic> diags, out EmitResult? result)
    {
        FailIfNotInitialized();
        var op = Begin("Compiling");
        var c = RoslynWorkspace.CurrentSolution
            .Projects.First()
            .GetCompilationAsync(Ct).Result;
        if (c is null)
        {
            op.Cancel();
            diags = Array.Empty<Diagnostic>();
            result = null;
            return false;
        }
        Action<Exception, DiagnosticAnalyzer, Diagnostic> errorHandler = (e, da, d) =>
        {
            Error(e, "Analyzer {0} threw an exception when reporting diagnostic {1}:", da.GetType().Name, d.Id);
        };
        var ca = c.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new SmartContractAnalyzer()), 
            new CompilationWithAnalyzersOptions(null, errorHandler, true, false, false, null));
        diags = ca.GetAllDiagnosticsAsync(Ct).Result;
        var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);
        if (File.Exists(TargetPath)) Warn("File {0} exists, overwriting...", ViewFilePath(TargetPath));
        using FileStream pestream = File.OpenWrite(TargetPath);
        using FileStream pdbstream = File.OpenWrite(Path.ChangeExtension(TargetPath, ".pdb")); 
        result = c.Emit(pestream, pdbstream, options: emitOptions);
        if(result is not null)
        {
            if (result.Success)
            {
                op.Complete();
                Info("Compilation succeded.");
                Info("Assembly is at {0}", TargetPath);               
            }
            else
            {
                op.Cancel();
                Error("Compilation failed.");
            }
            return result.Success;
        }
        else
        {
            op.Cancel();
            return false;
        }
    }

    public bool SscCompile(out SscCompilation? sscc)
    {
        FailIfNotInitialized();
        sscc = null;
        CSharpParseOptions parseOptions = new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.None, SourceCodeKind.Regular, DefineConstants.Split(';'));
        
        var op2 = Begin("Parsing and rewriting {0} files", SourceFiles.Count);
        IEnumerable<SyntaxTree> syntaxTrees = SourceFiles.Select(item => CSharpSyntaxTree.ParseText(File.ReadAllText(item), parseOptions, item, cancellationToken: Ct));
        foreach(var st in syntaxTrees)
        {
            var stdiags = st.GetDiagnostics(Ct);
            if (!stdiags.Any()) continue;
            LogDiagnostics(stdiags, ProjectFile.DirectoryName!);
            if (stdiags.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                return false;
            }
        }
        List<SyntaxTree> rewrittenSyntaxTrees = new();
        OriginalSourceFiles = new();
        foreach (var st in syntaxTrees)
        {
            SyntaxNode rwt = st.GetRoot(Ct);
            for (var i = 0; i < rewriters.Length; i++)
            {
                var rw = rewriters[i];
                var rwn = rewriterNames[i];
                var prev = rwt;
                var prevSt = prev.SyntaxTree;
                try
                {
                    rwt = rw.Visit(rwt);
                    var preText = prevSt.GetText();
                    foreach (var tc in rwt.SyntaxTree.GetChanges(prevSt))
                    {
                        if (string.IsNullOrEmpty(tc.NewText)) continue;
                        var flp = prevSt.GetLineSpan(tc.Span);
                        Info("Rewriter: {0}.", rwn);
                        Info("File: {0}", ViewFilePath(st.FilePath, ProjectFile.DirectoryName));
                        Info("Original: {0}", preText.ToString(tc.Span));
                        Info("New: {0}\n", tc.NewText);
                    }
                    var rwtDiags = rwt.GetDiagnostics();
                    LogDiagnostics(rwtDiags, Path.GetDirectoryName(st.FilePath)!);
                    if (rwtDiags.Any(d => d.Severity == 0))
                    {
                        Error("Error rewriting syntax of {0} using {1}. Skipping this rewriter.", st.FilePath, rw.GetType().Name);
                        rwt = prev;
                    }
                }
                catch (Exception ex)
                {
                    Error(ex, "Error rewriting syntax of {0} using {1}. Skipping this rewriter.", st.FilePath, rw.GetType().Name);
                    rwt = prev;
                }
            }
            rewrittenSyntaxTrees.Add(rwt.SyntaxTree);
            File.WriteAllText(Path.ChangeExtension(st.FilePath, ".ssc"), rwt.GetText().ToString());
            OriginalSourceFiles.Add(st.FilePath);
        }
        SourceFiles = syntaxTrees.Select(st => Path.ChangeExtension(st.FilePath, ".ssc")).ToList();
        op2.Complete();

        References.Insert(0, Path.Combine(AssemblyLocation, "Stratis.SmartContracts.NET4.dll"));
        
        using (var op = Parent is null ?  
            Begin("Compiling Spec# project using configuration {0}", BuildConfiguration!) : Begin("Compiling Spec# reference for project {0} using configuration {1}", Parent.ProjectFile.Name, BuildConfiguration!))
        {
            var compilerErrors = new List<SscCompilerError>();
            var compilerWarnings = new List<SscCompilerWarning>();
            Debug("ssc command-line: {0}.", CommandLine);
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
                }, isNETFxTool: true);

            if (output is null || output.Contains("error"))
            {
                Error("Compile failed.");
                op.Cancel();
                sscc = new SscCompilation(this, false, Verify, compilerErrors, compilerWarnings);
                return false;
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
                sscc = new SscCompilation(this, true, Verify, compilerErrors, compilerWarnings);
                return false;
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

    public static void LogDiagnostics(IEnumerable<Diagnostic> diags, string projDir)
    {
        if(diags.Count(d => d.WarningLevel == 0) > 0 || (DebugEnabled && diags.Count() > 0))
        {
            Info("Printing diagnostics...");
            foreach (var d in diags)
            {
                var f = d.Location.GetLineSpan().Path;
                var line = d.Location.GetLineSpan().StartLinePosition.Line;
                var col = d.Location.GetLineSpan().StartLinePosition.Character;
                if (d.WarningLevel == 0)
                {
                    Error("Id: {0}\n               Msg: {1}\n               File: {2}\n               Line: ({3},{4})\n", d.Id, d.GetMessage(), ViewFilePath(f, projDir), line, col);
                }
                else if (DebugEnabled)
                {
                    Warn("Id: {0}\n               Msg: {1}\n               File: {2}\n               Line: ({3},{4})\n", d.Id, d.GetMessage(), ViewFilePath(f, projDir), line, col);
                }
            }
        }
    }
    #endregion

    #region Fields
    private static CSharpSyntaxRewriter[] rewriters =
    {
        new BlockifyExpressionBody(),
        //new SharpSyntaxRewriter.Rewriters.DeanonymizeType(),
        //new SharpSyntaxRewriter.Rewriters.EmplaceGlobalStatement(),
        //new SharpSyntaxRewriter.Rewriters.EnsureVisibleConstructor(),
        //new SharpSyntaxRewriter.Rewriters.ExpandForeach(),
        //new SharpSyntaxRewriter.Rewriters.ImplementAutoProperty(),
        //new SharpSyntaxRewriter.Rewriters.ImposeThisPrefix(),
        //new SharpSyntaxRewriter.Rewriters.InitializeOutArgument(),
        //new SharpSyntaxRewriter.Rewriters.ReplicateLocalInitialization(),
        //new SharpSyntaxRewriter.Rewriters.StoreObjectCreation(),
        new TranslateLinq(),
        new UncoalesceCoalescedNull(),
        new UninterpolateString(),
        new NoNameof()
        //new SharpSyntaxRewriter.Rewriters.UnparameterizeRecordDeclaration()

    };

    private static string[] rewriterNames = rewriters.Select(r => r.GetType().Name).ToArray();
    #endregion
}