namespace Silver.CLI.Commands;

using Silver.CLI.Core;
using Silver.Verifier;

using static Program;

internal class CompilerCmd : Command
{
    internal static void GetProperty(string filePath, string buildConfig, string prop, params string[] additionalFiles)
    {
        ExitIfFileNotFound(filePath);
        if (!Compiler.PrintProperty(filePath, buildConfig, prop, additionalFiles))
        {
            Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {
            ExitWithSuccess();
        }
    }

    internal static void GetCommandLine(string filePath, string buildConfig, params string[] additionalFiles)
    {
        ExitIfFileNotFound(filePath);
        if (!Compiler.GetCommandLine(filePath, buildConfig))
        {
            Exit(ExitResult.ERROR_IN_RESULTS);
        }
        else
        {   
            ExitWithSuccess();
        }
    }

    internal static void Compile(string filePath, string buildConfig, bool verify, bool ssc, bool rewrite, bool validate, bool norewriteassert, bool noscanalyze, params string[] additionalFiles)
    {
        ExitIfFileNotFound(filePath);
        if (!Compiler.Compile(filePath, buildConfig, verify, ssc, rewrite, validate, norewriteassert, noscanalyze, out var target) || target is null)
        {
            Exit(ExitResult.UNKNOWN_ERROR);
        }
        else
        {
            if (verify)
            {
                var results = Boogie.Verify(target);
                if (results is null)
                {
                    Error("Could not read the verifier response.");
                    Exit(ExitResult.UNKNOWN_ERROR);
                }
                else
                {
                    Verifier.PrintVerifierResultsToConsole(results);
                }
            }
            ExitWithSuccess();
        }
    }
}