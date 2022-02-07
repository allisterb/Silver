namespace Silver;

using System.Reflection;

using Microsoft.Extensions.Configuration;

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

    public static bool DebugEnabled { get; set; }

    public static bool InteractiveConsole { get; set; } = false;

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

    public static Func<T, bool> All<T>() => (x_ => true);

    public static Func<T, T> Identity<T>() => (x => x);
    
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
    public static void Warn(string messageTemplate, params object[] args) => Logger.Warn(messageTemplate, args);

    [DebuggerStepThrough]
    public static void Fatal(string messageTemplate, params object[] args) => Logger.Fatal(messageTemplate, args);

    [DebuggerStepThrough]
    public static Logger.Op Begin(string messageTemplate, params object[] args) => Logger.Begin(messageTemplate, args);

    [DebuggerStepThrough]
    public static void WarnIfFileExists(string filename)
    {
        if (File.Exists(filename)) Warn("File {0} exists, overwriting...", filename);
    }

    [DebuggerStepThrough]
    public void FailIfNotInitialized()
    {
        if(!Initialized)
        {
            throw new RuntimeNotInitializedException(this);
        }
    }

    [DebuggerStepThrough]
    public T FailIfNotInitialized<T>(Func<T> r) => Initialized ? r() : throw new RuntimeNotInitializedException(this);

    public static string? RunCmd(string cmdName, string arguments = "", string? workingDir = null, DataReceivedEventHandler? outputHandler = null, DataReceivedEventHandler? errorHandler = null, 
        bool checkExists = true, bool isNETFxTool = false, bool isNETCoreTool = false)
    {
        if (checkExists && !(File.Exists(cmdName) || (isNETCoreTool && File.Exists(cmdName.Replace(".exe", "")))))
        {
            Error("The executable {0} does not exist.", cmdName);
            return null;
        }
        using (Process p = new Process())
        {
            var output = new StringBuilder();
            var error = new StringBuilder();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            
            if (isNETFxTool && System.Environment.OSVersion.Platform == PlatformID.Unix)
            {
                p.StartInfo.FileName = "mono";
                p.StartInfo.Arguments = cmdName + " " + arguments;
            }
            else if (isNETCoreTool && System.Environment.OSVersion.Platform == PlatformID.Unix)
            {
                p.StartInfo.FileName = File.Exists(cmdName) ? cmdName : cmdName.Replace(".exe", "");
                p.StartInfo.Arguments = arguments;

            }
            else
            {
                p.StartInfo.FileName = cmdName;
                p.StartInfo.Arguments = arguments;
            }
            
            p.OutputDataReceived += (sender, e) => 
            { 
                if (e.Data is not null) 
                { 
                    output.AppendLine(e.Data); 
                    Debug(e.Data);
                    outputHandler?.Invoke(sender, e);
                } 
            };
            p.ErrorDataReceived += (sender, e) => 
            { 
                if (e.Data is not null) 
                { 
                    error.AppendLine(e.Data); 
                    Error(e.Data);
                    errorHandler?.Invoke(sender, e);
                } 
            };
            if (workingDir is not null)
            {
                p.StartInfo.WorkingDirectory = workingDir;
            }
            Debug("Executing cmd {0} in working directory {1}.", cmdName + " " + arguments, p.StartInfo.WorkingDirectory);
            try
            {
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
                return error.ToString().IsNotEmpty() ? null : output.ToString();
            }
            
            catch (Exception ex)
            {
                Error(ex, "Error executing command {0} {1}", cmdName, arguments);
                return null;
            }
        }
    }

    public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = false)
    {
        using var op = Begin("Copying {0} to {1}", sourceDir, destinationDir);
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
        op.Complete();
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

    public static string ViewFilePath(string path, string? relativeTo = null)
    {
        if (!DebugEnabled)
        {
            if (relativeTo is null)
            {
                return (Path.GetFileName(path) ?? path);
            }
            else
            {
                return (Path.GetRelativePath(relativeTo, path));
            }
        }
        else return path;
    }
    #endregion
}
