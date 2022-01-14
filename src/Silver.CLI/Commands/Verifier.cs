namespace Silver.CLI.Commands;

using Silver.Core;
internal class VerifierCmd : Runtime
{
    public static void Verify(string path)
    {
        var r = Verifier.Verify(path);
        if (r is null)
        {
            Error("Could not read the verifier response.");
            Program.Exit(ExitResult.NOT_FOUND);
        }
        else
        {
            Verifier.PrintVerifierResultsToConsole(r);
            Program.Exit(ExitResult.SUCCESS);
        }
    }
}

