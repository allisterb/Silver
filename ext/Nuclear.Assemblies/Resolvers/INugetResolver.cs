using System.Collections.Generic;
using System.IO;

using Nuclear.Assemblies.ResolverData;

namespace Nuclear.Assemblies.Resolvers {

    /// <summary>
    /// A resolver that searches for a NuGet package.
    /// </summary>
    public interface INugetResolver : IAssemblyResolver<INugetResolverData> {

        #region properties

        /// <summary>
        /// Gets the matching strategy for package versions.
        /// </summary>
        VersionMatchingStrategies PackageMatchingStrategy { get; }

        /// <summary>
        /// Gets the used NuGet package cache directories.
        /// </summary>
        IEnumerable<DirectoryInfo> NugetCaches { get; }

        #endregion

    }

}
