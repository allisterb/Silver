namespace Silver.Verifier;

using System.Xml;
using System.Xml.Serialization;

using Spectre.Console;

using Silver.Verifier.Models;
public class Boogie : Runtime
{
    public static BoogieResults? Verify(string path)
    {

        var f = DateTime.Now.Ticks.ToString() + Path.GetFileName(FailIfFileNotFound(path)) + ".xml";
        Debug("XML output will be written to file {0}.", Path.GetFullPath(f));
        var ret = RunCmd(Path.Combine(AssemblyLocation, "ssc", "SscBoogie"), path + " " + "/xml:"+ f);
        if (ret is null)
        {
            Error("Coould not run program verifier.");
            return null;
        }
        else if(!ret.Contains("Spec# program verifier finished"))
        {
            Error("Program verifier did not complete successfully");
            return null;
        }
        else
        {
            XmlSerializer ser = new XmlSerializer(typeof(BoogieResults));
            using XmlReader reader = XmlReader.Create(f);
            var results =  (BoogieResults?)ser.Deserialize(reader);
            reader.Close();
            reader.Dispose();
            Debug("Deleting file {0}.", Path.GetFullPath(f));
            File.Delete(f);
            return results;
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
            var status = m.Conclusion.Outcome == "errors" ?  "[red]Failed[/]" : "[lime]Ok[/]";
            var method = methods.AddNode(($"[cyan]{m.Name.EscapeMarkup()}[/]: {status}"));
            if (m.Errors is not null)
            {
                foreach (var error in m.Errors)
                {
                    if (error.LineSpecified)
                    {
                        method.AddNode($"Line: {error.Line}");
                    }
                    if (error.ColumnSpecified)
                    {
                        method.AddNode($"Column: {error.Column}");
                    }
                    method.AddNode($"Message: {error.Message}");
                }
            }
            method.AddNode($"Duration: {m.Conclusion.Duration}s");
        }
        AnsiConsole.Write(tree);
    }
}

