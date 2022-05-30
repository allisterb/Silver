namespace Silver;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class ToolSourceSettings
{
    public string Name { get; set; } = String.Empty;
    public string Version { get; set; } = String.Empty;
    public Dictionary<string, string> DownloadURLs { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> ExePathsWithinZip { get; set; } = new Dictionary<string, string>();
    public string DependencyRelativePath { get; set; } = String.Empty;
    public string CommandPath { get; set; } = GetDefaultPath();

    public string Directory { get; set; } = string.Empty;
    
    private static string GetDefaultPath()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ToolSourceSettings))!.Location);
        return assemblyPath!.Split(new string[] { @".store" }, StringSplitOptions.None)[0];
    }
}
