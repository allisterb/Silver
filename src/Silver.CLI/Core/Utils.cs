using static Silver.Runtime;
using System.Net;

namespace Silver.CLI.Core
{
    internal static class Utils 
    {
        

        public static Markup ToMarkup(this string s) => new Markup(s);

    }
}
