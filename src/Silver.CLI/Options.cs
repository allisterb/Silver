using CommandLine;
using CommandLine.Text;

namespace Silver.CLI
{
    public class Options
    {
        [Option('d', "debug", Required = false, HelpText = "Enable debug mode.")]
        public bool Debug { get; set; }


        [Option('s', "script", Required = false, HelpText = "Enable script(non-interactive) mode.")]
        public bool Script { get; set; }

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

    [Verb("install", HelpText = "Install any required external tools.")]
    public class InstallOptions : Options 
    {
        [Option('i', "info", Required = false, HelpText = "Print version information for installed external tools.")]
        public bool Info { get; set; }
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

    [Verb("compile", HelpText = "")]
    public class CompileOptions : Options
    {
        [Value(0, Required = true, HelpText = "The source files or project file to compile.")]
        public IEnumerable<string> Files { get; set; } = Array.Empty<string>();

        [Option('v', "verify", Required = false, HelpText = "Verify the specified source files or project file after compilation.")]
        public bool Verify { get; set; } 

        [Option('p', "prop", Required = false, HelpText = "Print the compile-time value of a property for the specified source files or project file.")]
        public string Property { get; set; } = string.Empty;

        [Option('l', "cmd-line", Required = false, HelpText = "Print the Spec# compiler command-line for the specified source files project file.")]
        public bool CommandLine { get; set; }

        [Option('b', "build-config", Default = "Debug", Required = false, HelpText = "Set the build configuration for the source files or project. Defaults to 'Debug'.")]
        public string BuildConfig { get; set; } = string.Empty;
    }

    [Verb("translate", HelpText = "Translate a .NET bytecode assembly or project to Boogie.")]
    public class TranslateOptions : Options 
    {
        [Value(0, Required = true, HelpText = "The file to translate.")]
        public string File { get; set; } = String.Empty;
    }

    [Verb("verify", HelpText = "Verify a .NET assembly using Boogie.")]
    public class VerifyOptions : Options
    {
        [Value(0, Required = true, HelpText = "The .NET assembly file to translate.")]
        public string File { get; set; } = String.Empty;
    }

    [Verb("assembly", HelpText = "Analyze a .NET bytecode assembly.")]
    public class AssemblyOptions : Options
    {
        [Value(0, Required = true, HelpText = "The assembly file (*.dll or *.exe) to analyze.")]
        public string File { get; set; } = String.Empty;

        [Option('r', "references", Required = false, HelpText = "Print references.")]
        public bool References { get; set; }
    }

    [Verb("dis", HelpText = "Disassenble a .NET bytecode assembly.")]
    public class DisassemblerOptions : Options
    {
        [Value(0, Required = true, HelpText = "The assembly file (usually *.dll or *.exe) to disassemble.")]
        public string File { get; set; } = String.Empty;

        [Option('n', "noil", Required = false, HelpText = "Emit only C# code i.e. decompile the assembly.")]
        public bool NoIL { get; set; }

        [Option('k', "stack", Required = false, HelpText = "Emit code that retains a stack.")]
        public bool Stack { get; set; }
    }
}
