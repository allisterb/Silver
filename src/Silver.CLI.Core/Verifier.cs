namespace Silver.CLI.Core;

using System.Text.RegularExpressions;

using Spectre.Console;

using Silver.Compiler;
using Silver.Verifier;
using Silver.Verifier.Models;

public class Verifier : Runtime
{
    public static bool Verify(string path, string? classPattern, string? methodPattern, string? output)
    {
        var target = FailIfFileNotFound(path);
        if (SilverProject.HasProjectExtension(path))
        {
            if (!Compiler.Compile(path, "Debug", true, false, true, true, false, false, out target) || target is null)
            {
                Error("One or more errors occurred compiling project {0}.", path);
                return false;
            }
        }
        var results = Boogie.Verify(FailIfFileNotFound(target), output);
        if (results is null)
        {
            Error("Could not read the verifier response.");
            return false;
        }
        else
        {
            PrintVerifierResultsToConsole(results, classPattern, methodPattern);
            return true;
        }
    }

    public static void PrintVerifierResultsToConsole(BoogieResults results, string? _classPattern = null, string? _methodPattern = null, string? output = null)
    {
        Regex? classPattern = _classPattern is not null ? new Regex(_classPattern, RegexOptions.Compiled | RegexOptions.Singleline) : null;
        Regex? methodPattern = _methodPattern is not null ? new Regex(_methodPattern, RegexOptions.Compiled | RegexOptions.Singleline) : null;
        if (classPattern is not null) Info("Filtering verification output using class pattern {0}.", classPattern.ToString());
        if (methodPattern is not null) Info("Filtering verification output using method pattern {0}.", methodPattern.ToString());

        var tree = new Tree("Verification results");
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
            var method = methods.AddNode(($"[cyan]{m.Name.EscapeMarkup()}[/]: {status}"));
            if (m.Errors is not null && m.Errors.Any())
            {
                var errors = method.AddNode("[red]Errors[/]");
                foreach (var error in m.Errors)
                {
                    var e = errors.AddNode(error.Message.EscapeMarkup());
                    if (error.File is not null && error.File.EndsWith(".ssc"))
                    {
                        e.AddNode($"File: [blue]{error.File!.Replace(".ssc", ".cs").EscapeMarkup()}[/]");
                    }
                    else
                    {
                        e.AddNode($"File: [blue]{error.File?.EscapeMarkup() ?? ""}[/]");
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
        AnsiConsole.Write(tree);
        AnsiConsole.WriteLine("");
        var errorCount = results.File.Methods.Where(m => m.Conclusion.Outcome == "errors").Count();
        if (errorCount == 0)
        {
            Info("Verification succeded for {0} method(s).", methodCount);
        }
        else
        {
            Info("{0} out of {1} method(s) failed verification.", errorCount, methodCount);
        }
    }
}