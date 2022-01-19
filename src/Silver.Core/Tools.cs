namespace Silver.Core;

public class Tools : Runtime
{
    public static bool Boogie(params string[] args)
    {
        var ret = RunCmd(Path.Combine(AssemblyLocation, "ssc", "SscBoogie"), args.Aggregate((a, b) => a + " " + b));
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
                   Path.Combine(AssemblyLocation, "ssc", "ssc"),
                   args.Select(a => a.StartsWith("/") ? a.TrimStart('/').Insert(0, "-") : a).JoinWithSpaces(),
                   Path.Combine(AssemblyLocation, "ssc")
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
}

