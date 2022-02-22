using Silver.Compiler;

namespace Silver.CLI.Core
{
    public class Compiler : Runtime
    {
       public static bool PrintProperty(string filePath, string buildConfig, string prop, params string[] additionalFiles)
        {
            var p = SilverProject.GetProperty(FailIfFileNotFound(filePath), buildConfig, prop, additionalFiles);
            if (p is null)
            {
                Error("The property {0} does not exist or is null for the project file {1}.", prop, filePath);
                return false;
            }
            else
            {
                Info("The compile-time value of property {0} in build configuration {1} is {2}.", prop, buildConfig, p);
                return true;
            }
        }

        public static bool GetCommandLine(string filePath, string buildConfig, params string [] additionalFiles)
        {
            var l = SilverProject.GetProject(FailIfFileNotFound(filePath), buildConfig, additionalFiles)?.CommandLine;
            {
                if (l is null)
                {
                    Error("Could not get Spec# command line for project file {0}.", filePath);
                    return false;
                }
                else
                {
                    Info("Spec# compiler command-line is {0}.", "ssc " + l);
                    return true;
                }
            }
        }

        public static bool Compile(string filePath, string buildConfig, bool verify, bool ssc, bool rewrite, bool validate, bool noassertrewrite, bool noscanalyze, out string? targetPath, params string[] additionalFiles)
        {
            targetPath = null;
            var proj = SilverProject.GetProject(FailIfFileNotFound(filePath), buildConfig, additionalFiles);
            if (proj is not null && proj.Initialized)
            {
                proj.Verify = verify;
                if (ssc || Path.GetExtension(filePath).StartsWith(".ssc"))
                {
                    var sscret = proj.SscCompile(rewrite, noassertrewrite, out var sscc);
                    targetPath = proj.TargetPath;
                    return sscret;
                }
                var c = proj.Compile(noscanalyze, out var diags, out var result);
                targetPath = proj.TargetPath;
                SilverProject.LogDiagnostics(diags, proj.ProjectFile.Directory!.FullName);
                if (validate)
                {
                    var op = Begin("Validating {0} source file(s) using Stratis SCT tool", proj.SourceFiles.Count);
                    var ret = Tools.Sct(false, "validate", proj.SourceFiles.JoinWithSpaces());
                    if (ret is not null)
                    {
                        op.Complete();
                        var retl = ret.Split(Environment.NewLine);
                        if (ret.Contains("Compilation OK: True") && ret.Contains("Format Valid: True") && ret.Contains("Determinism Valid: True") &&
                            !retl.Any(r => r.StartsWith("Error: ")))
                        {
                            Info("SCT: Assembly is valid smart contract assembly.");
                        }
                        else if (!ret.Contains("Determinism Valid: False"))
                        {
                            foreach(var l in retl.Where(l => l.StartsWith("Error: ")))
                            {
                                Error("SCT: {0}.", l.Replace("Error: ", ""));
                            }
                        }
                        else
                        {
                            var deterrors = retl.SkipWhile(l => l != "Determinism Valid: False").Skip(1).Where(l => !(string.IsNullOrEmpty(l) || l.StartsWith("==")));
                            foreach (var error in deterrors)
                            {
                                Error("SCT: {0}", error);
                            }
                        }
                    }
                    else
                    {
                        op.Abandon();
                    }
                }
                if (c && verify)
                {
                    c = proj.SscCompile(rewrite, noassertrewrite, out var sscc);
                    targetPath = proj.TargetPath;
                    return c;
                }
                return c;
               
            }
            else
            {
                return false;
            }
        }
    }
}
