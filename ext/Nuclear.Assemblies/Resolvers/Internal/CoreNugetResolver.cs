using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Nuclear.Assemblies.Extensions;
using Nuclear.Assemblies.Factories;
using Nuclear.Assemblies.ResolverData;
using Nuclear.Assemblies.Runtimes;
using Nuclear.Creation;
using Nuclear.Extensions;

namespace Nuclear.Assemblies.Resolvers.Internal {
    internal class CoreNugetResolver : ICoreNugetResolver {

        #region fields

        private static readonly ICreator<INugetResolverData, FileInfo> _factory = Factory.Instance.NugetResolver();

        #endregion

        #region methods

        public IEnumerable<INugetResolverData> Resolve(
            AssemblyName assemblyName,
            IEnumerable<DirectoryInfo> cacheDirs,
            RuntimeInfo current,
            VersionMatchingStrategies assemblyMatchingStrategy,
            VersionMatchingStrategies packageMatchingStrategy) {

            List<INugetResolverData> files = new List<INugetResolverData>();

            if(RuntimesHelper.TryGetLoadableRuntimes(current, out IEnumerable<RuntimeInfo> validRuntimes)) {
                cacheDirs.Foreach(cache => files.AddRange(GetAssemblyCandidatesFromCache(assemblyName, cache, validRuntimes, assemblyMatchingStrategy, packageMatchingStrategy)));
            }

            return files
                .OrderByDescending(d => d.PackageVersion)
                .ThenByDescending(d => d.PackageTargetFramework, new RuntimeInfoFeatureComparer());
        }

        internal static IEnumerable<INugetResolverData> GetAssemblyCandidatesFromCache(
            AssemblyName assemblyName, DirectoryInfo cacheDir,
            IEnumerable<RuntimeInfo> validRuntimes,
            VersionMatchingStrategies assemblyMatchingStrategy,
            VersionMatchingStrategies packageMatchingStrategy) {

            List<INugetResolverData> files = new List<INugetResolverData>();

            if(TryGetPackage(assemblyName.Name, cacheDir, out DirectoryInfo packageDir)) {

                foreach(FileInfo file in packageDir.EnumerateFiles($"{assemblyName.Name}.dll", SearchOption.AllDirectories)) {

                    if(_factory.TryCreate(out INugetResolverData data, file)
                        && assemblyName.Name == data.AssemblyName.Name
                        && assemblyName.Version.Matches(data.AssemblyName.Version, assemblyMatchingStrategy)
                        && assemblyName.Version.Matches((Version) data.PackageVersion, packageMatchingStrategy)
                        && AssemblyHelper.ValidateArchitecture(data.AssemblyName)
                        && validRuntimes.Contains(data.PackageTargetFramework)) {

                        files.Add(data);
                    }
                }
            }

            return files
                .OrderByDescending(d => d.PackageVersion)
                .ThenByDescending(d => d.PackageTargetFramework, new RuntimeInfoFeatureComparer());
        }

        internal static Boolean TryGetPackage(String name, DirectoryInfo cache, out DirectoryInfo packageDir) {
            packageDir = null;

            try {
                packageDir = cache.EnumerateDirectories(name, SearchOption.TopDirectoryOnly).FirstOrDefault();

            } catch { /* Don't worry about exceptions here! */ }

            return packageDir != null && packageDir.Exists;
        }

        #endregion

    }
}
