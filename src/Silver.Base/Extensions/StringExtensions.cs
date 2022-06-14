namespace Silver;

public static class StringExtensions
{
    public static bool IsEmpty(this string s) => s == "";

    public static bool IsNotEmpty(this string s) => s != "";

    public static string ToAlphaNumeric(this string s) => new string(s.Where(c => Char.IsLetterOrDigit(c)).ToArray());


    //From https://stackoverflow.com/a/11278412 by user https://stackoverflow.com/users/14357/spender
    public static int ToInteger(this string numberString)
    {
        if (Int32.TryParse(numberString, out int _number))
        {
            return _number;
        }
        else
        {
            var numbers = Regex.Matches(numberString, @"\w+").Cast<Match>()
                    .Select(m => m.Value.ToLowerInvariant())
                    .Where(v => numberTable.ContainsKey(v))
                    .Select(v => numberTable[v]);
            int acc = 0, total = 0;
            foreach (var n in numbers)
            {
                if (n >= 1000)
                {
                    total += (acc * n);
                    acc = 0;
                }
                else if (n >= 100)
                {
                    acc *= n;
                }
                else acc += n;
            }
            return (total + acc) * (numberString.StartsWith("minus",
                    StringComparison.InvariantCultureIgnoreCase) ? -1 : 1);
        }
    }

    private static Dictionary<string, int> numberTable = new Dictionary<string, int>
    {{"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},
    {"five",5},{"six",6},{"seven",7},{"eight",8},{"nine",9},
    {"ten",10},{"eleven",11},{"twelve",12},{"thirteen",13},
    {"fourteen",14},{"fifteen",15},{"sixteen",16},
    {"seventeen",17},{"eighteen",18},{"nineteen",19},{"twenty",20},
    {"thirty",30},{"forty",40},{"fifty",50},{"sixty",60},
    {"seventy",70},{"eighty",80},{"ninety",90},{"hundred",100},
    {"thousand",1000},{"million",1000000}};

    public static Version? ToVersion(this string s) => Version.TryParse(s, out var v) ? v : null;

    public static string NormalizeFilePath(this string s) => s.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

    public static Stream ToStream(this string s)
    {
        return new MemoryStream(Encoding.Default.GetBytes(s));
    }

    public static bool HasPeExtension(this string s) =>
        Path.GetExtension(s) == ".dll" || Path.GetExtension(s) == ".exe";

    public static bool IsGitHubUrl(this string u) =>
        Uri.TryCreate(u, UriKind.Absolute, out var uri) && uri.Host == "github.com" ? true : false;
}

