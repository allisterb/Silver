namespace Silver.CLI.Core;

using Microsoft.Cci;

using Silver.CodeAnalysis.IL;
using Silver.Compiler;
using Silver.Drawing;
    
public class IL : Runtime
{
    public static bool Disassemble(string fileName, bool boogie, bool noIL, string? classPattern = null, string? methodPattern = null)
    {
        var targetAssembly = GetTargetAssembly(FailIfFileNotFound(fileName));
        if (targetAssembly is null) return false;
        if (boogie)
        {
            var output = Translator.ToBoogie(FailIfFileNotFound(targetAssembly));
            if (output is null)
            {
                Error("Could not disassemble {0} as  Boogie.", targetAssembly);
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (InteractiveConsole)
            {
                var output = new ColorfulConsoleSourceEmitterOutput();
                Disassembler.Run(targetAssembly, output, noIL, classPattern, methodPattern, true);
                return true;
            }
            else
            {
                var output = new CSharpSourceEmitter.SourceEmitterOutputString();
                Disassembler.Run(targetAssembly, output, noIL, classPattern, methodPattern);
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
            List<TreeNode> publicMethodNodes = new();
            List<TreeNode> notpublicMethodNodes = new();
            foreach(var method in summary.Methods
                .Where(m => m.ContainingTypeDefinition == c)
                .OrderByDescending(m => m.Visibility))
            {
                TreeNode? node = null;
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
            TreeNode[] nodes = publicMethodNodes.Concat(notpublicMethodNodes).ToArray();
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
        Graph.Draw(cg, outputFileName, graphFormat, rotateBy: Math.PI / 2);
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
        Graph.Draw(cfg, outputFileName, graphFormat);
        return true;
    }

    public static string? GetTargetAssembly(string f)
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
            if (proj.BuildUpToDate)
            {
                Info("Project {0} is up-to-date. Last build was on {1}.", ViewFilePath(f), File.GetLastWriteTime(proj.TargetPath));
                Info("Target assembly is {0}.", proj.TargetPath);
                return proj.TargetPath;
            }
            else if (proj.Compile(out var _, out var _))
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

