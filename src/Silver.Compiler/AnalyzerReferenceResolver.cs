namespace Silver.Compiler;

using Roslyn = Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;

public class AnalyzerAssemblyLoader : Roslyn.IAnalyzerAssemblyLoader
{   
    public Assembly LoadFromPath(string file)
    {
        if (assemblyReferences.ContainsKey(file))
        {
            return assemblyReferences[file];
        }
        else
        {
            var a = Assembly.LoadFrom(file);
            assemblyReferences.Add(file, a);
            return a;
        }
        
    }

    public Dictionary<string, Assembly> assemblyReferences = new();
    public void AddDependencyLocation(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException(path);
        assemblyReferences.AddIfNotExists(path, Assembly.LoadFrom(path));
    }
    public static AnalyzerAssemblyLoader Instance = new();


}

