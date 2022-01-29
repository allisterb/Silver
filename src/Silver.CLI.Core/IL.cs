using Silver.CodeAnalysis.IL;
using Silver.Projects;
using Silver.Drawing;

namespace Silver.CLI.Core
{
    public class IL : Runtime
    {
        public static string? GetTargetAssembly(string f)
        {
            if (f.HasPeExtension())
            {
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
                    Info("Project {0} is up-to-date.", f);
                    return proj.TargetPath;
                }
                else if (proj.NativeBuild())
                {
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
        public static bool Disassemble(string fileName, bool boogie, bool noIL, bool noStack)
        {
            var output =  boogie ? Translator.ToBoogie(FailIfFileNotFound(fileName)) : Disassembler.Run(FailIfFileNotFound(fileName), noIL, noStack);
            if (output is null)
            {
                Error("Could not disassemble {0}.", fileName);
                return false;
            }
            else
            {
                Con.WriteLine(output);
                return true;
            }
        }
        public static bool Summarize(string fileName, bool all)
        {
            var a = GetTargetAssembly(FailIfFileNotFound(fileName));
            if (a is null)
            {
                Error("Could not get target assembly to analyze.");
                return false;
            }
            
            var an = new Analyzer(a, all);
            if (!an.Initialized)
            {
                Error("Could not create an analyzer for {0}.", an);
                return false;
            }
            else
            {
                var s =  an.GetSummary();
                Info("Structs:{0}.", s.Fields);
                return true;
            }
            //var cfg = an.GetControlFlowGraphs();
                
            
        }
        public static bool PrintCallGraphs(string fileName, string outputFileName, string format, bool all)
        {
            if(!Enum.TryParse<GraphFormat>(format.ToUpper(), out var graphFormat))
            {
                Error("Invalid graph format: {0}.", format);
                return false;
            }
            var analyzer = GetAnalyzer(fileName, all);
            if (analyzer is null) return false;
            var cg = analyzer.GetCallGraph();
            if (cg is null) return false;
            Graph.Draw(cg, outputFileName, graphFormat);
            return true;
        }
            

        internal static Analyzer? GetAnalyzer(string fileName, bool all)
        {
            var a = GetTargetAssembly(FailIfFileNotFound(fileName));
            if (a is null)
            {
                Error("Could not get target assembly to analyze.");
                return null;
            }

            var an = new Analyzer(a, all);
            if (!an.Initialized)
            {
                Error("Could not create an analyzer for {0}.", an);
                return null;
            }
            else return an;
        }
    }
}
