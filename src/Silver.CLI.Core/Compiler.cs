using Silver.Projects;

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

        public static bool Compile(string filePath, string buildConfig, bool verify, bool ssc, bool rewrite, params string[] additionalFiles)
        {
            var proj = SilverProject.GetProject(FailIfFileNotFound(filePath), buildConfig, additionalFiles);
            if (proj is not null && proj.Initialized)
            {
                proj.Verify = verify;
                if (ssc)
                {
                    return proj.SscCompile(false, out var sscc);
                }
                var c = proj.Compile(out var diags, out var result);
                if (diags.Count(d => d.WarningLevel == 0) > 0 || (DebugEnabled && diags.Count() > 0))
                {
                    Info("Printing diagnostics...");
                    foreach (var d in diags)
                    {
                        var f = d.Location.GetLineSpan().Path;
                        var line = d.Location.GetLineSpan().StartLinePosition.Line;
                        var col = d.Location.GetLineSpan().StartLinePosition.Character;

                        if (d.WarningLevel == 0)
                        {
                            Error("Id: {0}\n               Msg: {1}\n               File: {2}\n               Line: ({3},{4})\n", d.Id, d.GetMessage(), ViewFilePath(f, proj.ProjectFile.Directory?.FullName), line, col);
                        }
                        else if (DebugEnabled)
                        {
                            Warn("Id: {0}\n               Msg: {1}\n               File: {2}\n               Line: ({3},{4})\n", d.Id, d.GetMessage(), ViewFilePath(f, proj.ProjectFile.Directory?.FullName), line, col);
                        }
                    }
                }
                if (verify)
                {
                    c = proj.SscCompile(rewrite, out var sscc);
                    return c;
                }
                else
                {
                    return c;
                }
                
            }
            else
            {
                return false;
            }
        }
    }
}
