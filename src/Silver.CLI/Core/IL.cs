namespace Silver.CLI.Core;

using Microsoft.Cci;
using Microsoft.Msagl.Drawing;

using Silver.CodeAnalysis.IL;
using Silver.Compiler;
using Silver.Drawing;
using Silver.Metadata;
using Silver.Verifier;

public class IL : Runtime
{
    public static bool Disassemble(DisassemblerOptions o)
    {
        var targetAssembly = GetTargetAssembly(FailIfFileNotFound(o.File), o.Boogie);
        if (targetAssembly is null) return false;
        if (o.Boogie)
        {
            var b = Boogie.Translate(targetAssembly, o.ClassPattern, o.MethodPattern);
            if (b is null)
            {
                return false;
            }
            else
            {
                var output = new StringBuilder();
                foreach(var l in b.Split(Environment.NewLine))
                {
                    if (!string.IsNullOrEmpty(l) && !l.StartsWith("type") && !l.StartsWith("const") && !l.StartsWith("function") && !l.StartsWith("axiom"))
                    {
                        output.AppendLine(l);
                    }
                }
                System.Console.WriteLine(o.ToString());
                return true;
            }
        }
        else if (o.TAC)
        {
            var a = IL.GetTargetAssembly(o.File);
            var an = new Analyzer(a);
            var mbs = an.GetMethodBodies();
            foreach (var m in mbs.Values)
            {
                Console.WriteLine(m.ToString());
            }
            return true;
        }
        else
        {
            if (InteractiveConsole)
            {
                var output = new ColorfulConsoleSourceEmitterOutput();
                Disassembler.Run(targetAssembly, output, o.NoIL, o.ClassPattern, o.MethodPattern, true);
                return true;
            }
            else
            {
                var output = new CSharpSourceEmitter.SourceEmitterOutputString();
                Disassembler.Run(targetAssembly, output, o.NoIL, o.ClassPattern, o.MethodPattern);
                System.Console.WriteLine(output.Data);
                return true;
            }
        }
    }
    public static bool Summarize(string fileName)
    {
        var an = GetAnalyzer(FailIfFileNotFound(fileName));
        if (an is null) return false;
        var summary =  an.GetSummary();
        var tree = new Tree("Summary");
        foreach (var c in summary.Classes)
        {
            var contract = tree.AddNode($"Contract [royalblue1]{c.GetName().EscapeMarkup()}[/]");
            var ctors = contract.AddNode("[yellow]Constructors[/]");
            var methods = contract.AddNode("[yellow]Methods[/]");
            List<Spectre.Console.TreeNode> publicMethodNodes = new();
            List<Spectre.Console.TreeNode> notpublicMethodNodes = new();
            foreach(var method in summary.Methods
                .Where(m => m.ContainingTypeDefinition == c)
                .OrderByDescending(m => m.Visibility))
            {
                Spectre.Console.TreeNode? node = null;
                if (method.Visibility == TypeMemberVisibility.Public)
                {
                    var parameters = method.Parameters is not null && method.Parameters.Any() ?
                    "[blue](" + method.Parameters.Select(p => p.Type.ToString()!.EscapeMarkup()).JoinWith(", ") + ")[/]"
                    : "[cyan]()[/]";
                    if (!method.IsConstructor)
                    {
                        node = methods.AddNode("[cyan]" + $"[{method.Visibility.ToString().ToLower()}] ".EscapeMarkup() + "[/]" + $"[cyan]{MemberHelper.GetMethodSignature(method).EscapeMarkup()}[/]{parameters}");
                    }
                    else
                    {
                        node = ctors.AddNode("[cyan]" + $"[{method.Visibility.ToString().ToLower()}] ".EscapeMarkup() + "[/]" + $"[cyan]{MemberHelper.GetMethodSignature(method).EscapeMarkup()}[/]{parameters}");
                    }
                    publicMethodNodes.Add(node);
                }
                else
                {
                    var parameters = method.Parameters is not null && method.Parameters.Any() ?
                    "[blue](" + method.Parameters.Select(p => p.Type.ToString()!.EscapeMarkup()).JoinWith(", ") + ")[/]"
                    : "[lime]()[/]";
                    if (!method.IsConstructor)
                    {
                        node = methods.AddNode("[lime]" + $"[{method.Visibility.ToString().ToLower()}] ".EscapeMarkup() + "[/]" + $"[lime]{MemberHelper.GetMethodSignature(method).EscapeMarkup()}[/]{parameters}");
                    }
                    else
                    {
                        node = ctors.AddNode("[lime]" + $"[{method.Visibility.ToString().ToLower()}] ".EscapeMarkup() + "[/]" + $"[lime]{MemberHelper.GetMethodSignature(method).EscapeMarkup()}[/]{parameters}");
                    }
                    notpublicMethodNodes.Add(node);

                }
                node.AddNode($"Parameters: [fuchsia]{method.ParameterCount}[/]");
                node.AddNode($"Instructions: [fuchsia]{method.Body.Operations.Count()}[/]");
                node.AddNode($"Gas cost: [fuchsia]{method.Body.Operations.GasCost()}[/]");
            }
            Spectre.Console.TreeNode[] nodes = publicMethodNodes.Concat(notpublicMethodNodes).ToArray();
            foreach(var node in nodes)
            {
                //node.AddNode("Parameters: {0}", me)
            }

        }
        Con.Write(tree);
        
        return true;
        
    }

    public static bool PrintCallGraphs(string fileName, string outputFileName, string format)
    {
        if(!Enum.TryParse<GraphFormat>(format.ToUpper(), out var graphFormat))
        {
            Error("Invalid graph format: {0}.", format);
            return false;
        }
        var analyzer = GetAnalyzer(fileName);
        if (analyzer is null) return false;
        var cg = analyzer.GetCallGraph();
        if (cg is null) return false;
        Drawing.Draw(cg.MsAglGraph, outputFileName, graphFormat, rotateBy: Math.PI / 2);
        return true;
    }
            
    public static bool PrintControlFlowGraph(string fileName, string outputFileName, string format)
    {
        if (!Enum.TryParse<GraphFormat>(format.ToUpper(), out var graphFormat))
        {
            Error("Invalid graph format: {0}.", format);
            return false;
        }
        var analyzer = GetAnalyzer(fileName);
        if (analyzer is null) return false;
        var cfg = analyzer.GetControlFlowGraph();
        if (cfg is null) return false;
        Drawing.Draw(cfg.MsAglGraph, outputFileName, graphFormat);
        return true;
    }

    public static string? GetTargetAssembly(string f, bool ssc = false)
    {
        if (f.HasPeExtension())
        {
            Info("Target assembly is {0}.", f);
            return f;
        }
        else if (SilverProject.HasProjectExtension(f))
        {
            var proj = SilverProject.GetProject(f, "Debug");
            if (proj is null || !proj.Initialized)
            {
                Error("Could not load project {0}.", f);
                return null;
            }
            //if (proj.BuildUpToDate)
            //{
            //    Info("Project {0} is up-to-date. Last build was on {1}.", ViewFilePath(f), File.GetLastWriteTime(proj.TargetPath));
            //    Info("Target assembly is {0}.", proj.TargetPath);
            //    return proj.TargetPath;
            //}
            if (!ssc && proj.Compile(false, out var _, out var _))
            {
                Info("Target assembly is {0}.", proj.TargetPath);
                return proj.TargetPath;
            }
            else if (ssc && proj.SscCompile(true, false, out var s))
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
                Error("Could not get target assembly for {0}.", f);
                return null;
            }
            Info("Target assembly is {0}.", fn);
            return fn;
        }
        else return null;
    }

    public static Analyzer? GetAnalyzer(string fileName)
    {
        var a = GetTargetAssembly(FailIfFileNotFound(fileName));
        if (a is null)
        {
            Error("Could not get target assembly for file {0} to analyze.", fileName);
            return null;
        }

        var an = new Analyzer(a);
        if (!an.Initialized)
        {
            Error("Could not create an analyzer for file {0}.", fileName);
            return null;
        }
        else return an;
    }
}

