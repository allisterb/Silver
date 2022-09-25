namespace Silver.Notebooks;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Silver.Compiler;
using Silver.CodeAnalysis.IL;
using Silver.Drawing;
using Silver.Metadata;
using Silver.Verifier;
using Silver.Verifier.Models;
public class Verifier : Runtime
{
    public static TreeDiagram? VerifyCode(string code, string _classPattern, string _methodPattern)
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
        var results =  Boogie.Verify(proj.TargetPath);
        File.Delete(proj.TargetPath);
        if (results is null)
        {
            return null;
        }
        Regex? classPattern = _classPattern is not null ? new Regex(_classPattern, RegexOptions.Compiled | RegexOptions.Singleline) : null;
        Regex? methodPattern = _methodPattern is not null ? new Regex(_methodPattern, RegexOptions.Compiled | RegexOptions.Singleline) : null;
        if (classPattern is not null) Info("Filtering verification output using class pattern {0}...", classPattern.ToString());
        if (methodPattern is not null) Info("Filtering verification output using method pattern {0}...", methodPattern.ToString());

        var tree = new TreeNode("Verification results");
        var file = tree.AddNode($"[royalblue1]File: {results.File.Name}[/]");
        var methods = file.AddNode("[yellow]Methods[/]");
        var methodCount = results.File.Methods.Length;
        foreach (var m in results.File.Methods)
        {
            var className = m.Name.Split('.').First();
            var methodName = m.Name.Split('.').Last().Split('$').First();
            if (classPattern is not null && !classPattern.IsMatch(className)) continue;
            if (methodPattern is not null && !methodPattern.IsMatch(methodName)) continue;

            var status = m.Conclusion.Outcome == "errors" ? "[red]Failed[/]" : "[lime]Ok[/]";
            var attr = new Dictionary<string, string>();
            var jstreeattr = new Dictionary<string, string>();
            jstreeattr["type"] = "demo";
            attr["data-jstree"] = JsonConvert.SerializeObject(jstreeattr);
            var method = methods.AddNode($"{m.Name}: {status}", attr);
            if (m.Errors is not null && m.Errors.Any())
            {
                var errors = method.AddNode("[red]Errors[/]");
                foreach (var error in m.Errors)
                {
                    var e = errors.AddNode(error.Message);
                    if (error.File is not null && error.File.EndsWith(".ssc"))
                    {
                        e.AddNode($"File: [blue]{error.File!.Replace(".ssc", ".cs")}[/]");
                    }
                    else
                    {
                        e.AddNode($"File: [blue]{error.File ?? ""}[/]");
                    }
                    if (error.LineSpecified)
                    {
                        e.AddNode($"Line: [fuchsia]{error.Line}[/]");
                    }
                    if (error.ColumnSpecified)
                    {
                        e.AddNode($"Column: [fuchsia]{error.Column}[/]");
                    }
                    //method.AddNode($"Message: {error.Message}");
                }
            }
            method.AddNode($"Duration: [fuchsia]{m.Conclusion.Duration}s[/]");
        }

        var errorCount = results.File.Methods.Where(m => m.Conclusion.Outcome == "errors").Count();
        if (errorCount == 0)
        {
            Info("Verification succeded for {0} method(s).", methodCount);
        }
        else
        {
            Info("{0} out of {1} method(s) failed verification.", errorCount, methodCount);
        }
        return new TreeDiagram(tree);
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

