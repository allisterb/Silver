namespace Silver.CLI;

using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

using Silver.CLI.Commands;

#region Enums
public enum ExitResult
{
    SUCCESS = 0,
    UNHANDLED_EXCEPTION = 1,
    INVALID_OPTIONS = 2,
    NOT_FOUND = 4,
    SERVER_ERROR = 5,
    ERROR_IN_RESULTS = 6,
    UNKNOWN_ERROR = 7
}
#endregion

class Program : Runtime
{
    #region Constructor
    static Program()
    {
        AppDomain.CurrentDomain.UnhandledException += Program_UnhandledException;
        InteractiveConsole = true;
        Console.CancelKeyPress += Console_CancelKeyPress;
        Console.OutputEncoding = Encoding.UTF8;
        foreach (var t in optionTypes)
        {
            optionTypesMap.Add(t.Name, t);
        }
    }
    #endregion

    #region Entry point
    static void Main(string[] args)
    {
        if (args.Contains("--debug") || args.Contains("-d"))
        {
            DebugEnabled = true;
            SetLogger(new SerilogLogger(console: true, debug: true));
            Info("Debug mode set.");
        }
        else
        {
            SetLogger(new SerilogLogger(console: true, debug: false));
        }

        PrintLogo();

        #region Parse options
        ParserResult<object> result = new Parser().ParseArguments<Options, CompileOptions, AnalyzeOptions, DisassemblerOptions,
            SummarizeOptions, CallGraphOptions, ControlFlowGraphOptions,
            VerifyOptions, AssemblyOptions,
            BoogieOptions, SscOptions, SctOptions, InstallOptions>(args);
        result.WithParsed<Options>(o =>
        {
            InteractiveConsole = !o.Script;
        })
        .WithParsed<InstallOptions>(o =>
        {
            Core.ExternalToolsManager.EnsureAllExists();
            if(o.Info)
            {
                foreach(var vi in Core.ExternalToolsManager.GetVersionInfo())
                {
                    Con.WriteLine(vi.Trim());
                }
            }
        })
        .WithParsed<BoogieOptions>(o =>
        {
            if (Core.Tools.Boogie(o.Options.ToArray()))
            {
                Exit(ExitResult.SUCCESS);
            }
            else
            {
                Exit(ExitResult.ERROR_IN_RESULTS);
            }
        })
        .WithParsed<SscOptions>(o =>
        {
            if (Core.Tools.Ssc(o.Options.ToArray()))
            {
                Exit(ExitResult.SUCCESS);
            }
            else
            {
                Exit(ExitResult.ERROR_IN_RESULTS);
            }

        })
        .WithParsed<SctOptions>(o =>
        {
            if (Core.Tools.Sct(o.Options.ToArray()))
            {
                Exit(ExitResult.SUCCESS);
            }
            else
            {
                Exit(ExitResult.ERROR_IN_RESULTS);
            }

        })
        .WithParsed<AssemblyOptions>(o =>
        {
            if (o.References)
            {
                AssemblyCmd.References(o.File);
                Exit(ExitResult.SUCCESS);
            }
        })
        .WithParsed<DisassemblerOptions>(o =>
        {
            ILCmd.Dissassemble(o.File, o.Boogie, o.NoIL, o.ClassPattern, o.MethodPattern);
        })
        .WithParsed<SummarizeOptions>(o =>
        {
            ExitIfFileNotFound(o.InputFile);
            ILCmd.Summarize(o.InputFile);

        })
          .WithParsed<CallGraphOptions>(o =>
          {
              ExitIfFileNotFound(o.InputFile);
              if (string.IsNullOrEmpty(o.OutputFormat)) o.OutputFormat = Path.GetExtension(o.OutputFile).TrimStart('.');
              ILCmd.PrintCallGraph(o);

          })
          .WithParsed<ControlFlowGraphOptions>(o =>
          {
              ExitIfFileNotFound(o.InputFile);

              if (string.IsNullOrEmpty(o.OutputFormat)) o.OutputFormat = Path.GetExtension(o.OutputFile).TrimStart('.');
              ILCmd.PrintControlFlowGraph(o);

          })
         .WithParsed<CompileOptions>(o =>
         {
             var file = o.Files.First();
             var additionalFiles = o.Files.Count() > 1 ? o.Files.Skip(1).ToArray() : Array.Empty<string>();
             var buildConfig = o.BuildConfig;
             if (!string.IsNullOrEmpty(o.Property))
             {
                 CompilerCmd.GetProperty(o.Files.First(), buildConfig, o.Property, additionalFiles);
             }
             else if (o.CommandLine)
             {
                 CompilerCmd.GetCommandLine(o.Files.First(), buildConfig, additionalFiles);
             }
             else
             {
                 CompilerCmd.Compile(file, buildConfig, o.Verify,o.Ssc, !o.Ssc, additionalFiles);
             }
         })
         .WithParsed<VerifyOptions>(o =>
         {
             ExitIfFileNotFound(o.File);
             VerifierCmd.Verify(o.File);
         })
        #endregion

        #region Print options help
        .WithNotParsed((IEnumerable<Error> errors) =>
        {
            HelpText help = GetAutoBuiltHelpText(result);
            help.Heading = new HeadingInfo("Silver", AssemblyVersion.ToString(3));
            help.Copyright = "";
            if (errors.Any(e => e.Tag == ErrorType.VersionRequestedError))
            {
                help.Heading = new HeadingInfo("Silver", AssemblyVersion.ToString(3));
                help.Copyright = new CopyrightInfo("Allister Beharry", new int[] { 2021, 2022 });
                Info(help);
                Exit(ExitResult.SUCCESS);
            }
            else if (errors.Any(e => e.Tag == ErrorType.HelpVerbRequestedError))
            {
                HelpVerbRequestedError error = (HelpVerbRequestedError)errors.First(e => e.Tag == ErrorType.HelpVerbRequestedError);
                if (error.Type != null)
                {
                    help.AddVerbs(error.Type);
                }
                else
                {
                    help.AddVerbs(optionTypes);
                }
                Info(help.ToString().Replace("--", ""));
                Exit(ExitResult.SUCCESS);
            }
            else if (errors.Any(e => e.Tag == ErrorType.HelpRequestedError))
            {
                HelpRequestedError error = (HelpRequestedError)errors.First(e => e.Tag == ErrorType.HelpRequestedError);
                help.AddVerbs(result.TypeInfo.Current);
                help.AddOptions(result);
                help.AddPreOptionsLine($"{result.TypeInfo.Current.Name.Replace("Options", "").ToLower()} options:");
                Info(help);
                Exit(ExitResult.SUCCESS);
            }
            else if (errors.Any(e => e.Tag == ErrorType.NoVerbSelectedError))
            {
                help.AddVerbs(optionTypes);
                Info(help);
                Exit(ExitResult.INVALID_OPTIONS);
            }
            else if (errors.Any(e => e.Tag == ErrorType.MissingRequiredOptionError))
            {
                MissingRequiredOptionError error = (MissingRequiredOptionError)errors.First(e => e.Tag == ErrorType.MissingRequiredOptionError);
                Error("A required option is missing: {0}.", error.NameInfo.NameText);
                Info(help);
                Exit(ExitResult.INVALID_OPTIONS);
            }
            else if (errors.Any(e => e.Tag == ErrorType.UnknownOptionError))
            {
                UnknownOptionError error = (UnknownOptionError)errors.First(e => e.Tag == ErrorType.UnknownOptionError);
                help.AddVerbs(optionTypes);
                Error("Unknown option: {error}.", error.Token);
                Info(help);
                Exit(ExitResult.INVALID_OPTIONS);
            }
            else
            {
                Error("An error occurred parsing the program options: {errors}.", errors);
                help.AddVerbs(optionTypes);
                Info(help);
                Exit(ExitResult.INVALID_OPTIONS);
            }
        });
        #endregion;
    }
    #endregion

    #region Methods
    public static void Exit(ExitResult result)
    {
        if (Cts != null && !Cts.Token.CanBeCanceled)
        {
            Cts.Cancel();
            Cts.Dispose();
        }
        RestoreOriginalConsoleColors();
        Serilog.Log.CloseAndFlush();
        Environment.Exit((int)result);
    }

    public static void ExitIfFileNotFound(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Error("The file {0} does not exist.", filePath);
            Exit(ExitResult.NOT_FOUND);
        }
    }

    public static void ExitIfNoTargetAssembly(string filePath)
    {
        ExitIfFileNotFound(filePath);
        if (Core.IL.GetTargetAssembly(filePath) is null)
        {
            Error("Could not get target assembly {0}.", filePath);
            Exit(ExitResult.NOT_FOUND);
        }
    }

    static void PrintLogo()
    {
        Con.Write(new FigletText(font, "Silver").LeftAligned().Color(Spectre.Console.Color.Cyan1));
        Con.Write(new Text($"v{AssemblyVersion.ToString(3)}\n").LeftAligned());
    }

    static void RestoreOriginalConsoleColors()
    {
        Console.ForegroundColor = originalConsoleForegroundColor;
        Console.BackgroundColor = originalConsoleBackgroundColor;
    }

    static HelpText GetAutoBuiltHelpText(ParserResult<object> result)
    {
        return HelpText.AutoBuild(result, h =>
        {
            h.AddOptions(result);
            return h;
        },
        e =>
        {
            return e;
        });
    }
    #endregion

    #region Event Handlers
    private static void Program_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Serilog.Log.CloseAndFlush();
        Error("Unhandled runtime error occurred. Silver CLI will now shutdown.");
        Con.WriteException((Exception) e.ExceptionObject);
        Exit(ExitResult.UNHANDLED_EXCEPTION);
    }

    private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        Info("Ctrl-C pressed. Exiting.");
        Cts.Cancel();
        Exit(ExitResult.SUCCESS);
    }
    #endregion

    #region Fields
    static object uilock = new object();
    static Type[] optionTypes = 
    { 
        typeof(Options), typeof(CompileOptions), typeof(AnalyzeOptions),
        typeof(SummarizeOptions), typeof(CallGraphOptions), typeof(ControlFlowGraphOptions),
        typeof(DisassemblerOptions), typeof(VerifyOptions), typeof(AssemblyOptions), 
        typeof(BoogieOptions), typeof(SscOptions), typeof(SctOptions), typeof(InstallOptions)

    };
    static FigletFont font = FigletFont.Load(Path.Combine(AssemblyLocation, "chunky.flf"));
    static Dictionary<string, Type> optionTypesMap = new Dictionary<string, Type>();

    static ConsoleColor originalConsoleForegroundColor = Console.ForegroundColor;
    static ConsoleColor originalConsoleBackgroundColor = Console.BackgroundColor;
    #endregion
}