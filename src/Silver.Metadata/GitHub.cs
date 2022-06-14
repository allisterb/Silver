namespace Silver.Metadata;

using System;

using Octokit;

public class GitHub : Runtime
{
    internal static async Task<PullRequest> GetStratisPR(int id)
    {
        var ghClient = new GitHubClient(new ProductHeaderValue("Silver", AssemblyVersion.ToString()));
        return await ghClient.PullRequest.Get("stratisproject", "CirrusSmartContracts", id);
    }

    internal static async Task<PullRequest> GetStratisPR(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)) throw new ArgumentException("Thr url is not valid.", "url");
        Int32.TryParse(uri.Segments.Last(), out int id);
        return await GetStratisPR(id);
    }

    public static byte[]? GetAssemblyByteCode(int id)
    {
        var pr = GetStratisPR(id);
        return null;
    }

    public static byte[]? GetAssemblyByteCode(string url)
    {
        var pr = GetStratisPR(url);
        return null;
    }
}

