namespace Silver.CLI.Core;

public class Tools : Runtime
{
    public static bool Boogie(params string[] args)
    {
        var ret = RunCmd(Path.Combine(AssemblyLocation, "ssc", "SscBoogie.exe"), args.Aggregate((a, b) => a + " " + b), isNETFxTool: true);
        if (ret is not null)
        {
            Info("Executed {0} command with args {1}.\n", "SscBooge", args.Aggregate((a, b) => a + " " + b));
            Con.Write($"[bold white]{ret.EscapeMarkup()}[/]".ToMarkup());
            return true;
        }
        else
        {
            Error("Error executing Boogie.");
            return false;
        }
    }

    public static bool Ssc(params string[] args)
    {
        var ret = RunCmd(
                   Path.Combine(AssemblyLocation, "ssc", "ssc.exe"),
                   args.Select(a => a.StartsWith("/") ? a.TrimStart('/').Insert(0, "-") : a).JoinWithSpaces(),
                   Path.Combine(AssemblyLocation, "ssc"),
                   isNETFxTool:true
               );
        if (ret is not null)
        {
            Info("Executed {0} command with args {1}.\n", "ssc", args.Select(a => a.StartsWith("/") ? a.TrimStart('/').Insert(0, "--") : a).JoinWithSpaces());
            Con.Write($"[bold white]{ret.EscapeMarkup()}[/]".ToMarkup());
            return true;
        }
        else
        {
            Error("Error executing ssc.");
            return false;
        }
    }

    public static string? Sct(bool echo = false, params string[] args)
    {
        var ret = RunCmd(
                   Path.Combine(AssemblyLocation, "sct", "Stratis.SmartContracts.Tools.Sct.exe"),
                   args.Select(a => a.StartsWith("/") ? a.TrimStart('/').Insert(0, "--") : a).JoinWithSpaces(),
                   Path.Combine(AssemblyLocation, "sct"), isNETCoreTool: true
               );
        if (ret is not null)
        {
            Info("Executed {0} command with args {1}.", "sct", args.Select(a => a.StartsWith("/") ? a.TrimStart('/').Insert(0, "--") : a).JoinWithSpaces());
            if (echo) Con.Write($"[bold white]{ret.EscapeMarkup()}[/]".ToMarkup());
        }
        else
        {
            Error("Error executing SCT too.");
        }
        return ret;
    }
}

