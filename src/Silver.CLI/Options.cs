using CommandLine;

namespace Silver.CLI;

#region Base classes
public class Options
{
    [Option("debug", Required = false, HelpText = "Enable debug mode.")]
    public bool Debug { get; set; }

    [Option("script", Required = false, HelpText = "Enable script (non-interactive) mode.")]
    public bool Script { get; set; }

    [Option("options", Required = false, HelpText = "Any additional options for the selected operation.")]
    public string AdditionalOptions { get; set; } = String.Empty;

    public static Dictionary<string, object> Parse(string o)
    {
        Dictionary<string, object> options = new Dictionary<string, object>();
        Regex re = new Regex(@"(\w+)\=([^\,]+)", RegexOptions.Compiled);
        string[] pairs = o.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in pairs)
        {
            Match m = re.Match(s);
            if (!m.Success)
            {
                options.Add("_ERROR_", s);
            }
            else if (options.ContainsKey(m.Groups[1].Value))
            {
                options[m.Groups[1].Value] = m.Groups[2].Value;
            }
            else
            {
                options.Add(m.Groups[1].Value, m.Groups[2].Value);
            }
        }
        return options;
    }
}

public class AnalyzeOptions : Options
{
    [Value(0, Required = true, HelpText = "The file to analyze.")]
    public string InputFile { get; set; } = String.Empty;
}
#endregion

#region Analyzers
[Verb("summarize", HelpText = "Summarize Stratis smart contracts in a .NET project or bytecode assembly.")]
public class SummarizeOptions : AnalyzeOptions { }

[Verb("cg", HelpText = "Get the call graph for Stratis smart contracts in a .NET project or bytecode assembly.")]
public class CallGraphOptions : AnalyzeOptions 
{
    [Value(1, Required = true, HelpText = "The output file for the operation. Format will be determined by the file extension or if specified by --format.")]
    public string OutputFile { get; set; } = String.Empty;

    [Option("format", Required = false, HelpText = "The format of the output file. Can be xml, svg, dot, dgml, png, or bmp.")]
    public string OutputFormat { get; set; } = String.Empty;
}

[Verb("cfg", HelpText = "Get the control-flow graphs for Stratis smart contracts in a .NET project or bytecode assembly.")]
public class ControlFlowGraphOptions : AnalyzeOptions
{
    [Value(1, Required = true, HelpText = "The output file for the operation. Format will be determined by the file extension or if specified by --format.")]
    public string OutputFile { get; set; } = String.Empty;

    [Option("format", Required = false, HelpText = "Output graphs in this format, otherwise use the file extension to determine format. Can be one of bmp, png, xml, dgml, svg.")]
    public string OutputFormat { get; set; } = String.Empty;

    [Option('t', "target", Required = false, HelpText = "Only output nodes and edges that have this name pattern as their target method.")]
    public string Target { get; set; } = String.Empty;

    [Option('s', "source", Required = false, HelpText = "Only output nodes and edges that have this name pattern as their source method.")]
    public string Source { get; set; } = String.Empty;
}
#endregion

#region Compiler
[Verb("compile", HelpText = "Compile a C# project file or source files.")]
public class CompileOptions : Options
{ 
    [Value(0, Required = true, HelpText = "The C# project file or source files to compile.")]
    public IEnumerable<string> Files { get; set; } = Array.Empty<string>();

    [Option('p', "prop", Required = false, HelpText = "Print the compile-time value of a property for the specified project file or source files.")]
    public string Property { get; set; } = string.Empty;

    [Option('l', "cmd-line", Required = false, HelpText = "Print the Spec# compiler command-line that will be invoked for the specified project file or source files.")]
    public bool CommandLine { get; set; }

    [Option('b', "build-config", Default = "Debug", Required = false, HelpText = "Set the build configuration for compiling the specified project file or source files. Defaults to 'Debug'.")]
    public string BuildConfig { get; set; } = string.Empty;

    [Option("validate", Required = false, HelpText = "Validate the assembly after compilation using the Stratis SCT tool.")]
    public bool Validate { get; set; }

    [Option('v', "verify", Required = false, HelpText = "Verify the specified project file or source files after compilation using Spec#.")]
    public bool Verify { get; set; }

    [Option("ssc", Required = false, HelpText = "Use the Spec# C# compiler instead of MSBuild to compile the project file or source files.")]
    public bool Ssc { get; set; }

    [Option("rewrite", Required = false, HelpText = "Rewrite the C# source files for compatibility with Spec#. This option is always true when --verify is specified.")]
    public bool Rewrite { get; set; } = false;

    [Option("no-assert-rw", Required = false, HelpText = "Don't rewrite smart contract Assert calls to their static Spec# equivalent.")]
    public bool NoAssertRewrite { get; set; } = false;

    [Option("no-sc-analyze", Required = false, HelpText = "Don't enable the smart contract Roslyn analyzer..")]
    public bool NoScAnalyze { get; set; } = false;

    [Option('c', "class", Required = false, HelpText = "If verifying only print verification results for methods belonging to classes matching this name pattern.")]
    public string? ClassPattern { get; set; }

    [Option('m', "method", Required = false, HelpText = "If verifying only print verification results for methods matching this name pattern.")]
    public string? MethodPattern { get; set; }
}
#endregion

#region Verifier
[Verb("verify", HelpText = "Verify a .NET assembly compiled with Spec#.")]
public class VerifyOptions : Options
{
    [Value(0, Required = true, HelpText = "The .NET assembly or project file to translate.")]
    public string File { get; set; } = String.Empty;

    [Option('c', "class", Required = false, HelpText = "Only print verification results for methods belonging to classes matching this name pattern.")]
    public string? ClassPattern { get; set; }

    [Option('m', "method", Required = false, HelpText = "Only print verification results for methods matching this name pattern.")]
    public string? MethodPattern { get; set; }

    [Option('o', "output", Required = false, HelpText = "Save the verifier XML output to this file.")]
    public string? Output { get; set; }
}
#endregion

#region Metadata
[Verb("metadata", HelpText = "Get metadata for a .NET bytecode assembly.")]
public class AssemblyOptions : Options
{
    [Value(0, Required = true, HelpText = "The assembly file (*.dll or *.exe) to analyze.")]
    public string File { get; set; } = String.Empty;

    [Option('r', "references", Required = false, HelpText = "Print references.")]
    public bool References { get; set; }
}
#endregion

#region Disassembler
[Verb("dis", HelpText = "Disassemble a .NET bytecode assembly.")]
public class DisassemblerOptions : Options
{
    [Value(0, Required = true, HelpText = "The .NET assembly file (usually *.dll or *.exe) to disassemble.")]
    public string File { get; set; } = String.Empty;

    [Option("cs", Required = false, HelpText = "Emit only C# code i.e. decompile.")]
    public bool NoIL { get; set; }

    [Option('c', "class", Required = false, HelpText = "Only disassemble methods belonging to classes matching this name pattern.")]
    public string? ClassPattern { get; set; }

    [Option('m', "method", Required = false, HelpText = "Only disassemble methods matching this name pattern.")]
    public string? MethodPattern { get; set; }

    [Option('b', "boogie", Required = false, HelpText = "Translate the assembly CIL to Boogie.")]
    public bool Boogie { get; set; }
}
#endregion

#region Tools
[Verb("boogie", HelpText = "Execute the installed Boogie tool with the specified options.")]
public class BoogieOptions : Options
{
    [Value(0, Required = true, HelpText = "The options to pass to Boogie.")]
    public IEnumerable<string> Options { get; set; } = Array.Empty<string>();
}

[Verb("ssc", HelpText = "Execute the installed Spec# compiler tool with the specified options.")]
public class SscOptions : Options
{
    [Value(0, Required = true, HelpText = "The options to pass to Boogie.")]
    public IEnumerable<string> Options { get; set; } = Array.Empty<string>();
}

[Verb("sct", HelpText = "Execute the installed Stratis smart contract tool with the specified options.")]
public class SctOptions : Options
{
    [Value(0, Required = true, HelpText = "The options to pass to Sct.")]
    public IEnumerable<string> Options { get; set; } = Array.Empty<string>();
}
#endregion

#region External Tools Manager
[Verb("install", HelpText = "Install any required external tools.")]
public class InstallOptions : Options
{
    [Option('i', "info", Required = false, HelpText = "Print version information for installed external tools.")]
    public bool Info { get; set; }
}
#endregion
