namespace Silver.CLI.Core;

using Spectre.Console;

using Silver.Compiler;
using Silver.Verifier;
using Silver.Verifier.Models;
public class Verifier : Runtime
{
    public static bool Verify(string path, string? output)
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
            PrintVerifierResultsToConsole(results);
            return true;
        }
    }

    public static void PrintVerifierResultsToConsole(BoogieResults results)
    {
        AnsiConsole.WriteLine("");
        var tree = new Tree("Verification results");
        var file = tree.AddNode($"[royalblue1]File: {results.File.Name}[/]");
        var methods = file.AddNode("[yellow]Methods[/]");
        foreach (var m in results.File.Methods)
        {
            var status = m.Conclusion.Outcome == "errors" ? "[red]Failed[/]" : "[lime]Ok[/]";
            var method = methods.AddNode(($"[cyan]{m.Name.EscapeMarkup()}[/]: {status}"));
            if (m.Errors is not null && m.Errors.Any())
            {
                var errors = method.AddNode("[red]Errors[/]");
                foreach (var error in m.Errors)
                {
                    var e = errors.AddNode(error.Message);
                    if (error.File!.EndsWith(".ssc"))
                    {
                        e.AddNode($"File: [blue]{error.File!.Replace(".ssc", ".cs").EscapeMarkup()}[/]");
                    }
                    else
                    {
                        e.AddNode($"File: [blue]{error.File!.EscapeMarkup()}[/]");
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
            method.AddNode($"Duration: {m.Conclusion.Duration}s");
        }
        AnsiConsole.Write(tree);
    }
}