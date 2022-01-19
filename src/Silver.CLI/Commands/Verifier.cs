namespace Silver.CLI.Commands;

using Silver.Core;
internal class VerifierCmd : Runtime
{
    public static void Verify(string path)
    {
        if (!Verifier.Verify(path))
        {
            Program.Exit(ExitResult.NOT_FOUND);
        }
        else
        {
            Program.Exit(ExitResult.SUCCESS);
        }
    }
}

