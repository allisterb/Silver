namespace Silver.Notebooks;

using Microsoft.Msagl.Drawing;

using Silver.CodeAnalysis.IL;
using Silver.Compiler;
using Silver.Drawing;

public class IL : Runtime
{
    public static string? GetTargetAssembly(string f)
    {
        if (f.HasPeExtension())
        {
            Info("Target assembly is {0}.", f);
            return f;
        }
        else if (SilverProject.HasProjectExtension(f))
        {
            var proj = SilverProject.GetProject(f, "Debug");
            if (proj is null || !proj.Initialized)
            {
                Error("Could not load project {0}.", f);
                return null;
            }
            if (proj.BuildUpToDate)
            {
                Info("Project {0} is up-to-date. Last build was on {1}.", ViewFilePath(f), File.GetLastWriteTime(proj.TargetPath));
                Info("Target assembly is {0}.", proj.TargetPath);
                return proj.TargetPath;
            }
            else if (proj.Compile(false, out var _, out var _))
            {
                Info("Target assembly is {0}.", proj.TargetPath);
                return proj.TargetPath;
            }
            else
            {
                Error("Could not build project {0}.", f);
                return null;
            }
        }
        else return null;
    }
    public static Analyzer? GetAnalyzer(string fileName)
    {
        var a = GetTargetAssembly(FailIfFileNotFound(fileName));
        if (a is null)
        {
            Error("Could not get target assembly for file {0} to analyze.", fileName);
            return null;
        }

        var an = new Analyzer(a);
        if (!an.Initialized)
        {
            Error("Could not create an analyzer for file {0}.", fileName);
            return null;
        }
        else return an;
    }

    public static Microsoft.Msagl.Drawing.Graph PrintCallGraphs(string fileName)
    {
        var analyzer = GetAnalyzer(fileName);
        return analyzer!.GetCallGraph();
    }
}
