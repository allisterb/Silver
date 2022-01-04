using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Nuclear.Assemblies.Factories;
using Nuclear.Assemblies.ResolverData;
using Nuclear.Assemblies.Runtimes;
using Nuclear.Creation;
using Nuclear.Exceptions;
using Nuclear.Extensions;

namespace Nuclear.Assemblies.Resolvers.Internal {
    internal class NugetResolver : AssemblyResolver<INugetResolverData>, INugetResolver {

        #region fields

        private static readonly ICreator<INugetResolverData, FileInfo> _factory = Factory.Instance.NugetResolver();

        private static readonly String _nugetDirName = ".nuget";

        private static readonly String _packagesDirName = "packages";

        #endregion

        #region properties

        public VersionMatchingStrategies PackageMatchingStrategy { get; }

        public IEnumerable<DirectoryInfo> NugetCaches { get; }

        internal ICoreNugetResolver CoreResolver { get; set; } = new CoreNugetResolver();

        #endregion

        #region ctors

        internal NugetResolver(VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy) : this(assemblyMatchingStrategy, packageMatchingStrategy, null) { }

        internal NugetResolver(VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy, IEnumerable<DirectoryInfo> caches) : base(assemblyMatchingStrategy) {
            Throw.IfNot.Enum.IsDefined<VersionMatchingStrategies>(packageMatchingStrategy, nameof(packageMatchingStrategy), $"Given strategy is not defined {packageMatchingStrategy.Format()}");

            PackageMatchingStrategy = packageMatchingStrategy;
            NugetCaches = caches ?? GetCaches();
        }

        #endregion

        #region public methods

        public override Boolean TryResolve(ResolveEventArgs e, out IEnumerable<INugetResolverData> data) {
            data = Enumerable.Empty<INugetResolverData>();

            return AssemblyHelper.TryGetAssemblyName(e, out AssemblyName assemblyName) && TryResolve(assemblyName, out data);
        }

        public override Boolean TryResolve(String fullName, out IEnumerable<INugetResolverData> data) {
            data = Enumerable.Empty<INugetResolverData>();

            return AssemblyHelper.TryGetAssemblyName(fullName, out AssemblyName assemblyName) && TryResolve(assemblyName, out data);
        }

        public override Boolean TryResolve(AssemblyName assemblyName, out IEnumerable<INugetResolverData> data) {
            data = Enumerable.Empty<INugetResolverData>();

            if(assemblyName != null && RuntimesHelper.TryGetCurrentRuntime(out RuntimeInfo current)) {
                data = CoreResolver.Resolve(assemblyName, NugetCaches, current, AssemblyMatchingStrategy, PackageMatchingStrategy);
            }

            return data != null && data.Count() > 0;
        }

        #endregion

        #region internal methods

        internal static IEnumerable<DirectoryInfo> GetCaches() {
            List<DirectoryInfo> caches = new List<DirectoryInfo>();

            DirectoryInfo userProfileDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            DirectoryInfo usersDir = userProfileDir.Parent;

            String userNugetDir = Path.Combine(userProfileDir.FullName, _nugetDirName, _packagesDirName);

            if(Directory.Exists(userNugetDir)) {
                caches.Add(new DirectoryInfo(userNugetDir));
            }

            foreach(DirectoryInfo userDir in usersDir.EnumerateDirectories()) {
                try {
                    String secondaryNugetDir = Path.Combine(userDir.FullName, _nugetDirName, _packagesDirName);

                    if(Directory.Exists(secondaryNugetDir) && secondaryNugetDir != userNugetDir) {
                        caches.Add(new DirectoryInfo(secondaryNugetDir));
                    }

                } catch { /* Don't care if we can't access drectories */ }

            }

            return caches;
        }

        #endregion

    }
}
