namespace Silver.CLI
{
    internal class Command : Runtime
    {
        protected static void ExitWithSuccess() => Program.Exit(ExitResult.SUCCESS);
        protected static void ExitIfFileNotExists(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Error("The file {0} does not exist.", fileName);
                Program.Exit(ExitResult.NOT_FOUND);
            }
        }
    }
}
