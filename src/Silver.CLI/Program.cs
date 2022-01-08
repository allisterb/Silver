namespace Silver.CLI;

using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

using Silver;
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
        Interactive = true;
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
            SetLogger(new SerilogLogger(console: true, debug: true));
            Info("Debug mode set.");
        }
        else
        {
            SetLogger(new SerilogLogger(console: true, debug: false));
        }

        PrintLogo();

        #region Parse options
        ParserResult<object> result = new Parser().ParseArguments<Options, InstallOptions, AssemblyOptions, BoogieOptions, SpecSharpOptions, TranslateOptions>(args);
        result.WithParsed<Options>(o =>
        {
            Interactive = !o.Script;
        })
        .WithParsed<InstallOptions>(o =>
        {
            ExternalToolsManager.EnsureAllExists();
            if(o.Info)
            {
                foreach(var vi in ExternalToolsManager.GetVersionInfo())
                {
                    Con.WriteLine(vi);
                }
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
        .WithParsed<BoogieOptions>(o =>
        {
            var ret = RunCmd("boogie", o.Options.Aggregate((a, b) => a + " " + b));
            if (ret is not null)
            {
                Con.Write($"[bold white]{ret.EscapeMarkup()}[/]".ToMarkup());
            }
            else
            {
                Error("Error executing Boogie.");
            }
        })
        .WithParsed<SpecSharpOptions>(o =>
        {
            var buildConfig = o.BuildConfig;
            if (o.Compile)
            {
                SscCmd.Compile(
                    o.Options.First(),
                    buildConfig
                );
            }
            else if(!string.IsNullOrEmpty(o.Property))
            {
                SscCmd.GetProperty(o.Options.First(), buildConfig, o.Property);
            }
            else if (o.CommandLine)
            {
                SscCmd.GetCommandLine(o.Options.First(), buildConfig);
            }
            else
            {
                var ret = RunCmd(
                    Path.Combine(AssemblyLocation, "ssc", "ssc"), 
                    o.Options.Select(a => a.StartsWith("/") ? a.TrimStart('/').Insert(0, "-") : a).JoinWithSpaces(), 
                    Path.Combine(AssemblyLocation, "ssc")
                );
                if (ret is not null)
                {
                    Con.Write($"[bold white]{ret.EscapeMarkup()}[/]".ToMarkup());
                }
                else
                {
                    Error("Error executing ssc.");
                }
            }
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
    static void PrintLogo()
    {
        Con.Write(new FigletText(font, "Silver").LeftAligned().Color(Spectre.Console.Color.Cyan1));
        Con.Write(new Text($"v{AssemblyVersion.ToString(3)}\n").LeftAligned());
    }

    public static void Exit(ExitResult result)
    {
        if (Cts != null && !Cts.Token.CanBeCanceled)
        {
            Cts.Cancel();
            Cts.Dispose();
        }
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
    static Type[] optionTypes = { typeof(Options), typeof(InstallOptions), typeof(AssemblyOptions), typeof(BoogieOptions), typeof(SpecSharpOptions), typeof(TranslateOptions) };
    static FigletFont font = FigletFont.Load(Path.Combine(AssemblyLocation, "chunky.flf"));
    static Dictionary<string, Type> optionTypesMap = new Dictionary<string, Type>();
    #endregion
}

