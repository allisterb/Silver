using CommandLine;
using CommandLine.Text;

namespace Silver.CLI
{
    #region Base classes
    public class Options
    {
        [Option('d', "debug", Required = false, HelpText = "Enable debug mode.")]
        public bool Debug { get; set; }

        [Option('s', "script", Required = false, HelpText = "Enable script (non-interactive) mode.")]
        public bool Script { get; set; }

        [Option('o', "output", Required = false, HelpText = "The output file (if any) for the operation.")]
        public string OutputFile { get; set; } = string.Empty;

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
        public string File { get; set; } = String.Empty;

        [Option('a', "all", Required = false, HelpText = "Analyze all classes in an assembly, not only contract classes.")]
        public bool All { get; set; }

        [Option("print-cfg", Required = false, HelpText = "Print the control-flow graphs for contract classes in the assembly.")]
        public bool PrintCFG { get; set; }
    }
    #endregion

    #region Analyzer
    [Verb("summarize", HelpText = "Summarize Stratis smart contracts in a .NET project or bytecode assembly.")]
    public class SummarizeOptions : AnalyzeOptions { }

    [Verb("call-graph", HelpText = "Get the call graph for Stratis smart contracts in a .NET project or bytecode assembly.")]
    public class CallGraphOptions : AnalyzeOptions { }
    #endregion

    [Verb("compile", HelpText = "Compile a C# source code project or files.")]
    public class CompileOptions : Options
    {
        [Value(0, Required = true, HelpText = "The project file or source files to compile.")]
        public IEnumerable<string> Files { get; set; } = Array.Empty<string>();

        [Option("sc", Required = false, HelpText = "Compile as a smart contract.")]
        public bool SmartContract { get; set; }

        [Option('v', "verify", Required = false, HelpText = "Verify the specified source files or project file after compilation.")]
        public bool Verify { get; set; } 

        [Option('p', "prop", Required = false, HelpText = "Print the compile-time value of a property for the specified source files or project file.")]
        public string Property { get; set; } = string.Empty;

        [Option('l', "cmd-line", Required = false, HelpText = "Print the Spec# compiler command-line for the specified source files project file.")]
        public bool CommandLine { get; set; }

        [Option('b', "build-config", Default = "Debug", Required = false, HelpText = "Set the build configuration for the source files or project. Defaults to 'Debug'.")]
        public string BuildConfig { get; set; } = string.Empty;
    }

    [Verb("verify", HelpText = "Verify a .NET assembly compiled with Spec#.")]
    public class VerifyOptions : Options
    {
        [Value(0, Required = true, HelpText = "The .NET assembly or project file to translate.")]
        public string File { get; set; } = String.Empty;
    }

    [Verb("metadata", HelpText = "Get metadata for a .NET bytecode assembly.")]
    public class AssemblyOptions : Options
    {
        [Value(0, Required = true, HelpText = "The assembly file (*.dll or *.exe) to analyze.")]
        public string File { get; set; } = String.Empty;

        [Option('r', "references", Required = false, HelpText = "Print references.")]
        public bool References { get; set; }
    }

    [Verb("dis", HelpText = "Disassemble a .NET bytecode assembly.")]
    public class DisassemblerOptions : Options
    {
        [Value(0, Required = true, HelpText = "The assembly file (usually *.dll or *.exe) to disassemble.")]
        public string File { get; set; } = String.Empty;

        [Option('n', "noil", Required = false, HelpText = "Emit only C# code i.e. decompile the assembly.")]
        public bool NoIL { get; set; }

        [Option('k', "stack", Required = false, HelpText = "Emit code that retains a stack.")]
        public bool Stack { get; set; }

        [Option('b', "boogie", Required = false, HelpText = "Translate the assembly bytecode to Boogie.")]
        public bool Boogie { get; set; }
    }

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

    [Verb("install", HelpText = "Install any required external tools.")]
    public class InstallOptions : Options
    {
        [Option('i', "info", Required = false, HelpText = "Print version information for installed external tools.")]
        public bool Info { get; set; }
    }
}
