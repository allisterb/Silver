using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Nuclear.Assemblies.ResolverData;
using Nuclear.Assemblies.Runtimes;

namespace Nuclear.Assemblies.Resolvers.Internal {
    internal interface ICoreNugetResolver {

        #region methods

        IEnumerable<INugetResolverData> Resolve(
            AssemblyName assemblyName,
            IEnumerable<DirectoryInfo> cacheDirs,
            RuntimeInfo current,
            VersionMatchingStrategies assemblyMatchingStrategy,
            VersionMatchingStrategies packageMatchingStrategy);

        #endregion

    }
}
