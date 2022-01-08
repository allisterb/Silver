namespace Silver;

using System.Net;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Spectre.Console;

public abstract class Runtime
{
    #region Constructors
    static Runtime()
    {
        Logger = new ConsoleLogger();
        IsKubernetesPod = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KUBERNETES_PORT")) || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENSHIFT_BUILD_NAMESPACE"));
        if (IsKubernetesPod)
        {
            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
        }
        else if (Assembly.GetEntryAssembly()?.GetName().Name == "Silver.CLI" && Environment.GetEnvironmentVariable("USERNAME") == "Allister")
        {
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets("c0697968-04fe-49d7-a785-aaa817e38935")
                .AddEnvironmentVariables()
                .Build();
        }
        else if (Assembly.GetEntryAssembly()?.GetName().Name == "Silver.CLI")
        {
            Configuration = new ConfigurationBuilder()
            .AddJsonFile("config.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        }
        else
        {
            Configuration = new ConfigurationBuilder()
            .AddJsonFile("config.json", optional: true)
            .Build();
        }

        DefaultHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Silver/" + AssemblyVersion.ToString(2));
    }
    public Runtime(CancellationToken ct)
    {
        Ct = ct;
    }
    public Runtime() : this(Cts.Token) { }
    #endregion

    #region Properties
    public static IConfigurationRoot Configuration { get; set; }

    public static string Config(string i) => Configuration[i];

    public static bool Interactive { get; set; } = false;

    public static bool IsKubernetesPod { get; }

    public static bool IsAzureFunction { get; set; }

    public static string PathSeparator { get; } = Environment.OSVersion.Platform == PlatformID.Win32NT ? "\\" : "/";

    public static Logger Logger { get; protected set; }

    public static CancellationTokenSource Cts { get; } = new CancellationTokenSource();

    public static CancellationToken Ct { get; protected set; } = Cts.Token;

    public static HttpClient DefaultHttpClient { get; } = new HttpClient();

    public static string AssemblyLocation { get; } = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Runtime))!.Location)!;

    public static Version AssemblyVersion { get; } = Assembly.GetAssembly(typeof(Runtime))!.GetName().Version!;

    public bool Initialized { get; protected set; }
    #endregion

    #region Methods
    public static void SetLogger(Logger logger)
    {
        Logger = logger;
    }

    public static void SetLoggerIfNone(Logger logger)
    {
        if (Logger == null)
        {
            Logger = logger;
        }
    }

    public static void SetDefaultLoggerIfNone()
    {
        if (Logger == null)
        {
            Logger = new ConsoleLogger();
        }
    }

    [DebuggerStepThrough]
    public static void Info(string messageTemplate, params object[] args) => Logger.Info(messageTemplate, args);

    [DebuggerStepThrough]
    public static void Debug(string messageTemplate, params object[] args) => Logger.Debug(messageTemplate, args);

    [DebuggerStepThrough]
    public static void Error(string messageTemplate, params object[] args) => Logger.Error(messageTemplate, args);

    [DebuggerStepThrough]
    public static void Error(Exception ex, string messageTemplate, params object[] args) => Logger.Error(ex, messageTemplate, args);

    [DebuggerStepThrough]
    public static Logger.Op Begin(string messageTemplate, params object[] args) => Logger.Begin(messageTemplate, args);

    [DebuggerStepThrough]
    public static T GetTimed<T>(Func<T> p, string status, string messageTemplate, params object[] o)
    {
        T ret;
        using (var op = Begin(messageTemplate, o))
        {
            ret = Interactive ? AnsiConsole.Status().Spinner(Spinner.Known.Dots).Start($"{status}...", ctx => p()) : p();
            op.Complete();
        }
        return ret;
    }

    public void FailIfNotInitialized()
    {
        if(!Initialized)
        {
            throw new RuntimeNotInitializedException(this);
        }
    }

    public T FailIfNotInitialized<T>(Func<T> r) => Initialized ? r() : throw new RuntimeNotInitializedException(this);

    public static string? RunCmd(string cmdName, string arguments = "", string? workingDir = null)
    {
        if (!File.Exists(cmdName))
        {
            Error("The executable {0} does not exist.", cmdName);
            return null;
        }
        using (Process p = new Process())
        {
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = cmdName;
            p.StartInfo.Arguments = arguments;
            if (workingDir is not null)
            {
                p.StartInfo.WorkingDirectory = workingDir;
            }
            try
            {
                p.Start();
                p.WaitForExit();
                string outputBinary = p.StandardOutput.ReadToEnd();
                string errorMsg = p.StandardError.ReadToEnd();
                
                if (!String.IsNullOrEmpty(errorMsg))
                {
                    Error(errorMsg);
                    return null;
                }
                else
                {
                    Debug("Command {0} {1} returned {2}", cmdName, arguments, outputBinary.Trim());
                    return outputBinary.Trim();
                }
            }
            
            catch (Exception ex)
            {
                Error(ex, "Error executing command {0} {1}", cmdName, arguments);
                return null;
            }
            finally
            {
                if (p.StandardOutput.EndOfStream)
                {
                    p.StandardOutput.Close();
                    p.StandardError.Close();
                }
            }
        }
    }

    public static void DownloadFile(string name, Uri downloadUrl, string downloadPath)
    {
        #pragma warning disable SYSLIB0014 // Type or member is obsolete
        using (var op = Begin("Downloading {0} from {1} to {2}", name, downloadUrl, downloadPath))
        {
            using (var client = new WebClient())
            {
                if (Interactive)
                {
                    AnsiConsole.Progress().Start(ctx =>
                    {
                        var task = ctx.AddTask($"[bold white]Download[/] [bold cyan]{downloadUrl}[/]");
                        client.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
                        {
                            task.Value = 100.0 * ((double) e.BytesReceived / (double) e.TotalBytesToReceive);
                        };
                        client.DownloadDataCompleted += (object sender, DownloadDataCompletedEventArgs e) =>
                        {
                            task.StopTask();
                        };
                        client.DownloadFileAsync(downloadUrl, downloadPath);
                        while (!task.IsFinished);
                    });
                    op.Complete();
                }
                else
                {
                    client.DownloadFile(downloadUrl, downloadPath);
                    op.Complete();
                }
            }
           
        }
        #pragma warning restore SYSLIB0014 // Type or member is obsolete
    }

    [DebuggerStepThrough]
    public static void SetProps<T>(T o, Dictionary<string, object> setprops)
    {
        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public |  BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var kv in setprops)
        {
            properties.Where(x => x.Name == kv.Key).First().SetValue(o, kv.Value);
        }
    }

    [DebuggerStepThrough]
    public static object? GetProp<T>(T o, string name)
    {
        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return properties.FirstOrDefault(x => x.Name == name)?.GetValue(o); 
    }

    [DebuggerStepThrough]
    public static string FailIfFileNotFound(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);
        return filePath;
    }
    #endregion
}
