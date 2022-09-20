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

    public static TmHighlightedCode? TranslateCode(string code, string? classname, string? methodname, bool allcode = false)
    {
        var tempFilePath = Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".cs";
        using var op = Begin("Compiling code to temporary assembly {0}", Path.GetFullPath(Path.GetFileNameWithoutExtension(tempFilePath) + ".dll"));
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
            var b = Boogie.Translate(proj.TargetPath, classname, methodname);
            if (b is not null)
            {
                if (allcode)
                {
                    return new TmHighlightedCode("boogie", b);
                }
                else
                {
                    var o = new StringBuilder();
                    foreach (var l in b.Split(System.Environment.NewLine))
                    {
                        if (!string.IsNullOrEmpty(l) && !l.StartsWith("type") && !l.StartsWith("const") && !l.StartsWith("function") && !l.StartsWith("axiom"))
                        {
                            o.AppendLine(l);
                        }
                    }
                    return new TmHighlightedCode("boogie", o.ToString());
                }
            }
            else
            {
                return null;
            }
        }
        else
        {
            op.Abandon();
            return null;
        }
    }
}

