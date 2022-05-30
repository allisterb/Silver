namespace Silver;

using System.IO;

using Microsoft.Extensions.Configuration;
    
public class ExternalToolsManager : Runtime
{
    #region Properties
    public static IConfiguration ToolSourceConfig { get; } = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine("toolsourcesettings.json"), false, false)
            .Build();
    
    public static ToolManager Z3 { get; private set; }

    public static ToolManager SpecSharp { get; private set; }

    public static ToolManager Sct { get; private set; }
    #endregion

    #region Methods
    public static void Init(IInterface managerInterface)
    {
        var z3SourceSettings = new ToolSourceSettings();
        ToolSourceConfig.GetSection("z3").Bind(z3SourceSettings);
        Z3 = new DownloadedToolManager(z3SourceSettings, managerInterface);

        var specsharpSourceSettings = new ToolSourceSettings();
        ToolSourceConfig.GetSection("specsharp").Bind(specsharpSourceSettings);
        SpecSharp = new DownloadedDotNetToolManager(specsharpSourceSettings, managerInterface);

        var sctSettings = new ToolSourceSettings();
        ToolSourceConfig.GetSection("sct").Bind(sctSettings);
        Sct = new DownloadedDotNetToolManager(sctSettings, managerInterface);
    }
    
    public static void EnsureAllExists()
    {
        Z3.EnsureExists();
        SpecSharp.EnsureExists();
        Sct.EnsureExists();
        Info("All required external tools installed.");
    }

    public static string[] GetVersionInfo()
    {
        return new string[] 
        { 
            Runtime.RunCmd(Path.Combine(AssemblyLocation, "z3.exe"), "/version") ?? "Error running z3 /version.",
            "Spec# 1.0.21126 (https://github.com/allisterb/specsharp-ms)", 
        };
    }
    #endregion
}

