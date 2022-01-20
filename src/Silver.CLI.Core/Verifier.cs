namespace Silver.CLI.Core;

using Spectre.Console;

using Silver.Verifier;
using Silver.Verifier.Models;
public class Verifier : Runtime
{
    public static bool Verify(string path)
    {
        var results = Boogie.Verify(FailIfFileNotFound(path));
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
                    if (error.LineSpecified)
                    {
                        e.AddNode($"Line: {error.Line}");
                    }
                    if (error.ColumnSpecified)
                    {
                        e.AddNode($"Column: {error.Column}");
                    }
                    //method.AddNode($"Message: {error.Message}");
                }
            }
            method.AddNode($"Duration: {m.Conclusion.Duration}s");
        }
        AnsiConsole.Write(tree);
    }
}