namespace Silver;

using Microsoft.Extensions.Configuration;
    
public static class ExternalToolsManager
{
    private static Logger Logger;

    public static IConfiguration ToolSourceConfig = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine("ExternalToolsManager", "toolsourcesettings.json"), false, false)
            .Build();
    public static ToolManager Z3 { get; private set; }
    
    public static ToolManager Boogie { get; private set; }
    
    public static ToolManager Corral { get; private set; }
    
    static ExternalToolsManager()
    {
        Logger = new ConsoleLogger();
        var z3SourceSettings = new ToolSourceSettings();
        ToolSourceConfig.GetSection("z3").Bind(z3SourceSettings);
        Z3 = new DownloadedToolManager(z3SourceSettings);

        var boogieSourceSettings = new ToolSourceSettings();
        ToolSourceConfig.GetSection("boogie").Bind(boogieSourceSettings);
        Boogie = new DotnetCliToolManager(boogieSourceSettings);

        var corralSourceSettings = new ToolSourceSettings();
        ToolSourceConfig.GetSection("corral").Bind(corralSourceSettings);
        Corral = new DotnetCliToolManager(corralSourceSettings);        
    }

    public static void EnsureAllExists()
    {
        Z3.EnsureExists();

        Boogie.EnsureExists();
        ((DotnetCliToolManager) Boogie).EnsureLinkedToZ3(Z3);

        Corral.EnsureExists();
        ((DotnetCliToolManager) Corral).EnsureLinkedToZ3(Z3);
        
        Runtime.Info("All required external tools installed.");
    }

    public static string[] GetVersionInfo()
    {
        return new string[] 
        { 
            Runtime.RunCmd("z3", "/version") ?? "Error running z3 /version.", 
            Runtime.RunCmd("boogie", "/version") ?? "Error running boogie /version.", 
            (Runtime.RunCmd("corral") ?? "Error running corral.").Split(Environment.NewLine).First()
        };
    }
}

