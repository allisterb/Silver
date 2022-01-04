using System;
using System.Reflection;

using Nuclear.Assemblies.Resolvers;
using Nuclear.Assemblies.Runtimes;
using Nuclear.SemVer;

namespace Nuclear.Assemblies.ResolverData {

    /// <summary>
    /// Defines the nuget package information that was found by an <see cref="INugetResolver"/>.
    /// </summary>
    public interface INugetResolverData : IAssemblyResolverData {

        #region properties

        /// <summary>
        /// Gets the package name.
        /// </summary>
        String PackageName { get; }

        /// <summary>
        /// Gets the package version.
        /// </summary>
        SemanticVersion PackageVersion { get; }

        /// <summary>
        /// Gets the tergeted processor architecture.
        /// </summary>
        ProcessorArchitecture PackageArchitecture { get; }

        /// <summary>
        /// Gets the targeted framework.
        /// </summary>
        RuntimeInfo PackageTargetFramework { get; }

        #endregion

    }

}
