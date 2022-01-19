using Microsoft.Cci;

using Silver.CodeAnalysis.IL;
using Silver.Projects;

namespace Silver.Core
{
    public class IL : Runtime
    {
        public static string? GetTargetAssembly(string f)
        {
            if (f.HasPeExtension())
            {
                return f;
            }
            else if (SpecSharpProject.HasProjectExtension(f))
            {
                var proj = SpecSharpProject.GetProject(f, "Debug");
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
        public static string? Disassemble(string fileName, bool boogie, bool noIL, bool noStack) =>
            boogie ? Translator.ToBoogie(FailIfFileNotFound(fileName)) : Disassembler.Run(FailIfFileNotFound(fileName), noIL, noStack);

        public static void PrintCfg(string fileName, bool all)
        {
            var a = GetTargetAssembly(FailIfFileNotFound(fileName));
            if (a is null)
            {
                Error("Could not get target assembly to analyze.");
                return;
            }
            
            var an = new Analyzer(a, all);
            if (!an.Initialized)
            {
                Error("Could not create an analyzer for {0}.", an);
                return;
            }
            (var types, var methods) = an.GetSummary();
            foreach (var t in types)
            {
                Con.WriteLine(t.GetName());
            }
            //var cfg = an.GetControlFlowGraphs();
                
            
        }
            
    }
}
