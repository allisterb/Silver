namespace Silver.Notebooks;

using System.IO;
using System.Text;
using Microsoft.Msagl.Drawing;

using Silver.Compiler;
using Silver.CodeAnalysis.IL;
using Silver.Drawing;
using Silver.Metadata;

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
        else if (f.IsGitHubUrl())
        {
            var fn = GitHub.GetAssemblyFromStratisPR(f);
            if (fn is null)
            {
                Error("Could not get target assembly.");
                return null;
            }
            Info("Target assembly is {0}.", fn);
            return fn;
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

    public static MermaidLanguage Draw(Summary summary)
    {
        var builder = new StringBuilder();
        builder.AppendLine("classDiagram");
        return new MermaidLanguage(builder.ToString());
    }
}
