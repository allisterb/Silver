namespace Silver.Notebooks;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Silver.Compiler;
using Silver.CodeAnalysis.IL;
using Silver.Drawing;
using Silver.Metadata;
using Silver.Verifier;
using Silver.Verifier.Models;
public class Verifier : Runtime
{
    public static string? GetTargetAssembly(string f)
    {
        if (f.HasPeExtension())
        {
            Info("Target assembly is {0}.", f);
            return f;
        }
        else if (f.HasProjectExtension())
        {
            var proj = SilverProject.GetProject(f, "Debug");
            if (proj is null || !proj.Initialized)
            {
                Error("Could not load project {0}.", f);
                return null;
            }
            if (proj.BuildUpToDate)
            {
                Info("Project {0} is up-to-date. Last build was on {1}.", ViewFilePath(f), File.GetLastWriteTime(proj.TargetPath));
                Info("Target assembly is {0}.", proj.TargetPath);
                return proj.TargetPath;
            }
            else if (proj.Compile(false, out var _, out var _))
            {
                Info("Target assembly is {0}.", proj.TargetPath);
                return proj.TargetPath;
            }
            else
            {
                Error("Could not build project {0}.", f);
                return null;
            }
        }
        else if (f.IsGitHubUrl())
        {
            var fn = GitHub.GetAssemblyFromStratisPR(f);
            if (fn is null)
            {
                Error("Could not get target assembly.");
                return null;
            }
            Info("Target assembly is {0}.", fn);
            return fn;
        }
        else
        {
            return null;
        }
    }

    public static BoogieResults? VerifyCode(string code)
    {
        var tempFilePath = Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".cs";
        using var op = Begin("Compiling code to temporary assembly {0}", Path.GetFileNameWithoutExtension(tempFilePath) + ".dll");
        File.WriteAllText(tempFilePath, code);
        var sourceFiles = new List<string>() { tempFilePath };
        var settings = new Dictionary<string, object>
                {
                    { "BuildConfig", "Debug" },
                    { "SourceFiles", sourceFiles }
                };
        var proj = new AdhocSilverProject(settings);
        proj.SscCompile(true, false, out var ssc);
        File.Delete(tempFilePath);
        if (ssc is not null && ssc.Succeded)
        {
            op.Complete();
        }
        else
        {
            op.Abandon();
            return null;
        }
        var r =  Boogie.Verify(proj.TargetPath);
        File.Delete(proj.TargetPath);
        return r;
    }
}

