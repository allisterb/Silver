namespace Silver.CLI.Core;

public class Tools : Runtime
{
    public static bool Boogie(params string[] args)
    {
        var ret = RunCmd(Path.Combine(AssemblyLocation, "ssc", "SscBoogie.exe"), args.Aggregate((a, b) => a + " " + b), isNETFxTool: true);
        if (ret is not null)
        {
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
            Con.Write($"[bold white]{ret.EscapeMarkup()}[/]".ToMarkup());
            return true;
        }
        else
        {
            Error("Error executing ssc.");
            return false;
        }
    }

    public static bool Sct(params string[] args)
    {
        var ret = RunCmd(
                   Path.Combine(AssemblyLocation, "sct", "Stratis.SmartContracts.Tools.Sct.exe"),
                   args.Select(a => a.StartsWith("/") ? a.TrimStart('/').Insert(0, "--") : a).JoinWithSpaces(),
                   Path.Combine(AssemblyLocation, "sct")
               );
        if (ret is not null)
        {
            Con.Write($"[bold white]{ret.EscapeMarkup()}[/]".ToMarkup());
            return true;
        }
        else
        {
            Error("Error executing sct.");
            return false;
        }
    }
}

