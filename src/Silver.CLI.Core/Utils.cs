using static Silver.Runtime;
using System.Net;

namespace Silver.CLI.Core
{
    internal static class Utils 
    {
        [DebuggerStepThrough]
        public static T GetTimed<T>(Func<T> p, string status, string messageTemplate, params object[] o)
        {
            T ret;
            using (var op = Runtime.Begin(messageTemplate, o))
            {
                ret = InteractiveConsole ? AnsiConsole.Status().Spinner(Spinner.Known.Dots).Start($"{status}...", ctx => p()) : p();
                op.Complete();
            }
            return ret;
        }

        public static void DownloadFile(string name, Uri downloadUrl, string downloadPath)
        {
        #pragma warning disable SYSLIB0014 // Type or member is obsolete
            using (var op = Begin("Downloading {0} from {1} to {2}", name, downloadUrl, downloadPath))
            {
                using (var client = new WebClient())
                {
                    if (InteractiveConsole)
                    {
                        AnsiConsole.Progress().Start(ctx =>
                        {
                            var task = ctx.AddTask($"[bold white]Download[/] [bold cyan]{downloadUrl}[/]");
                            client.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
                            {
                                task.Value = 100.0 * ((double)e.BytesReceived / (double)e.TotalBytesToReceive);
                            };
                            client.DownloadDataCompleted += (object sender, DownloadDataCompletedEventArgs e) =>
                            {
                                task.StopTask();
                            };
                            client.DownloadFileAsync(downloadUrl, downloadPath);
                            while (!task.IsFinished) ;
                        });
                        op.Complete();
                    }
                    else
                    {
                        client.DownloadFile(downloadUrl, downloadPath);
                        op.Complete();
                    }
                }

            }
        #pragma warning restore SYSLIB0014 // Type or member is obsolete
        }

        public static Markup ToMarkup(this string s) => new Markup(s);

    }
}
