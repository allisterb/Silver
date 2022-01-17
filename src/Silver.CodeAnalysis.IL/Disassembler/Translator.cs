namespace Silver.CodeAnalysis.IL;

public class Translator : Runtime
{
    public static string? ToBoogie(string fileName, params string[] args)
    {
        using (var op = Begin("Translating .NET assembly bytecode"))
        {
            var translatorErrors = new List<string>();
            var temp = Path.Combine(AssemblyLocation, Path.GetRandomFileName());
            var a = new string[] { "-i", fileName, "-o", temp };
            var output = RunCmd(Path.Combine(AssemblyLocation, "TinyBCT.NET6"), a.Concat(args).JoinWithSpaces(), outputHandler: (sender, e) =>
            {
                if (e.Data is not null && (e.Data.Contains("Unhandled exception") || e.Data.ToLower().Contains("error")))
                {
                    translatorErrors.Add(e.Data);
                    Error(e.Data);
                }
            });
            var o = Path.ChangeExtension(temp, "bpl");
            if (output is null || translatorErrors.Any())
            {
                Error("Translation failed.");
                if (File.Exists(o)) File.Delete(o);
                op.Cancel();
                return null;
            }
            else
            {
                var bpl = File.ReadAllText(o);
                File.Delete(o);
                op.Complete();
                return bpl;
            }
        }
    }
}

