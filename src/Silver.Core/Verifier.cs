namespace Silver.Core;

using Silver.Verifier;
using Silver.Verifier.Models;
public class Verifier : Runtime
{
    public static BoogieResults? Verify(string path) => Boogie.Verify(FailIfFileNotFound(path));

    public static void PrintVerifierResultsToConsole(BoogieResults results) => Boogie.PrintVerifierResultsToConsole(results);
}

