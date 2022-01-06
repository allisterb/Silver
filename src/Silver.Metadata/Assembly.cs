namespace Silver.Metadata;

using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableContracts;

using Nuclear.Assemblies;
using Nuclear.Assemblies.Resolvers;
using Nuclear.Assemblies.ResolverData;
using Nuclear.Assemblies.Resolvers.Internal;
public readonly record struct AssemblyReference(AssemblyName Name, IAssemblyResolverData? ResolverData);
public class Assembly : Runtime
{
    #region Constructors
    public Assembly(string assemblyPath)
    { 
        Directory = new FileInfo(assemblyPath).Directory!;
        Host = new CodeContractAwareHostEnvironment(new string[] { Directory.FullName }, true, true);
        Module = (IModule)Host.LoadUnitFrom(assemblyPath);
        References = Module.AssemblyReferences.Select(a => new AssemblyReference(GetAssemblyName(a), TryResolve(a, Directory.FullName)));
    }
    #endregion

    #region Properties
    public DirectoryInfo Directory { get; init; }
    public CodeContractAwareHostEnvironment Host { get; init; }
    public IModule Module { get; init; }

    public IEnumerable<AssemblyReference> References { get; init; }
  
    internal static DefaultResolver DefaultResolver { get; } = new DefaultResolver(VersionMatchingStrategies.SemVer, SearchOption.AllDirectories);
    internal static NugetResolver NugetResolver {get; } = new NugetResolver(VersionMatchingStrategies.SemVer, VersionMatchingStrategies.SemVer);
    #endregion

    #region Methods
    public static AssemblyName GetAssemblyName(IAssemblyReference r)
    {
        var name = new AssemblyName(r.Name.Value)
        {
            Version = r.Version,
            CultureName = r.Culture,
        };
        name.SetPublicKey(r.PublicKey.Any() ? r.PublicKey.ToArray() : null);
        name.SetPublicKeyToken(r.PublicKeyToken.Any() ? r.PublicKeyToken.ToArray() : null);
        return name;
    }
    public static IAssemblyResolverData? TryResolve(AssemblyName name, string searchPath)
    {
        var defaultResolverData = System.IO.Directory.Exists(searchPath) ? DefaultResolver.CoreResolver.Resolve(name, new DirectoryInfo(searchPath), SearchOption.AllDirectories, VersionMatchingStrategies.Strict) : null;
        if (defaultResolverData is not null && defaultResolverData.Any())
        {
            Debug("Resolved assembly {0} using default resolver.", name);
            return defaultResolverData.First();
        }
        else
        {
            NugetResolver.TryResolve(name, out var nugetResolverData);
            if (nugetResolverData is not null && nugetResolverData.Any())
            {
                Debug("Resolved assembly {0} using NuGet resolver.", name);
                return nugetResolverData.First();
            }
            else
            {
                return null;
            }   
        }
    }

    public static IAssemblyResolverData? TryResolve(IAssemblyReference r, string searchPath) => TryResolve(GetAssemblyName(r), searchPath);
    
    #endregion
}

