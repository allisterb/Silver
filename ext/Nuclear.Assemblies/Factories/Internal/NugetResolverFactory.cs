using System;
using System.Collections.Generic;
using System.IO;

using Nuclear.Assemblies.ResolverData;
using Nuclear.Assemblies.ResolverData.Internal;
using Nuclear.Assemblies.Resolvers;
using Nuclear.Assemblies.Resolvers.Internal;
using Nuclear.Creation;

namespace Nuclear.Assemblies.Factories.Internal {

    internal class NugetResolverFactory : Factories.NugetResolverFactory {

        #region fields

        private static readonly ICreator<INugetResolver, VersionMatchingStrategies, VersionMatchingStrategies> _resolverCreator =
            Factory.Instance.Creator.Create<INugetResolver, VersionMatchingStrategies, VersionMatchingStrategies>((in1, in2) => new NugetResolver(in1, in2));

        private static readonly ICreator<INugetResolver, VersionMatchingStrategies, VersionMatchingStrategies, IEnumerable<DirectoryInfo>> _resolverWithCacheCreator =
            Factory.Instance.Creator.Create<INugetResolver, VersionMatchingStrategies, VersionMatchingStrategies, IEnumerable<DirectoryInfo>>((in1, in2, in3) => new NugetResolver(in1, in2, in3));

        private static readonly ICreator<INugetResolverData, FileInfo> _dataCreator =
            Factory.Instance.Creator.Create<INugetResolverData, FileInfo>((file) => new NugetResolverData(file));

        #endregion

        #region methods

        public override void Create(out INugetResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy)
            => _resolverCreator.Create(out obj, assemblyMatchingStrategy, packageMatchingStrategy);

        public override Boolean TryCreate(out INugetResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy)
            => _resolverCreator.TryCreate(out obj, assemblyMatchingStrategy, packageMatchingStrategy);

        public override Boolean TryCreate(out INugetResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy, out Exception ex)
            => _resolverCreator.TryCreate(out obj, assemblyMatchingStrategy, packageMatchingStrategy, out ex);

        public override void Create(out INugetResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy, IEnumerable<DirectoryInfo> caches)
            => _resolverWithCacheCreator.Create(out obj, assemblyMatchingStrategy, packageMatchingStrategy, caches);

        public override Boolean TryCreate(out INugetResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy, IEnumerable<DirectoryInfo> caches)
            => _resolverWithCacheCreator.TryCreate(out obj, assemblyMatchingStrategy, packageMatchingStrategy, caches);

        public override Boolean TryCreate(out INugetResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy, IEnumerable<DirectoryInfo> caches, out Exception ex)
            => _resolverWithCacheCreator.TryCreate(out obj, assemblyMatchingStrategy, packageMatchingStrategy, caches, out ex);

        public override void Create(out INugetResolverData obj, FileInfo file)
            => _dataCreator.Create(out obj, file);

        public override Boolean TryCreate(out INugetResolverData obj, FileInfo file)
            => _dataCreator.TryCreate(out obj, file);

        public override Boolean TryCreate(out INugetResolverData obj, FileInfo file, out Exception ex)
            => _dataCreator.TryCreate(out obj, file, out ex);

        #endregion

    }

}
