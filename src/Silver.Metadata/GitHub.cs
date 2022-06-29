namespace Silver.Metadata;

using System;
using System.IO;
using Octokit;

public class GitHub : Runtime
{
    internal static async Task<PullRequest?> GetStratisPR(int id)
    {
        try
        {
            var ghClient = new GitHubClient(new ProductHeaderValue("Silver", AssemblyVersion.ToString()));
            return await ghClient.PullRequest.Get("stratisproject", "CirrusSmartContracts", id);
        }
        catch (Exception ex)
        {
            Error(ex, "Error getting Stratis pull request {0} from GitHub.", id);
            return null;
        }
    }

    internal static async Task<PullRequest?> GetStratisPR(string url)
    {
        if (!url.IsGitHubUrl() || !Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            Error("{0} is not a valid GitHub url.", url);
            return null;
        }
        if (!Int32.TryParse(uri.Segments.Last(), out int id))
        {
            Error("Could not decipher Stratis pull request id from url {0}.", url);
            return null;
        }
        return await GetStratisPR(id);
    }


    public static string? GetAssemblyFromStratisPR(string url)
    {
        using var op = Begin("Get smart contract assembly from Stratis pull request {0}", url);
        var pr = GetStratisPR(url).Result;
        if (pr == null) return null;
        var s = pr.Body.Split("**Contract Byte Code**");
        if (s == null || s.Length != 2)
        {
            Error("Could not find contract byte code section in pull request body.");
            op.Abandon();
            return null;
        }
        var c = s[1].Replace("`", "").Replace("\r", "").Replace("\n", "").Replace("\"", "").Replace("\'", "");
        var b = Enumerable.Range(0, c.Length / 2).Select(x => Convert.ToByte(c.Substring(x * 2, 2), 16)).ToArray();
        var fn = "Stratis.PR" + pr.Id + ".dll";
        WarnIfFileExists(fn);
        File.WriteAllBytes(fn, b);
        op.Complete();
        return fn;
    }
}

