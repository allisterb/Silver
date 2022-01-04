using System;
using System.IO;

using Nuclear.Assemblies.ResolverData;
using Nuclear.Assemblies.ResolverData.Internal;
using Nuclear.Assemblies.Resolvers;
using Nuclear.Assemblies.Resolvers.Internal;
using Nuclear.Creation;

namespace Nuclear.Assemblies.Factories.Internal {

    internal class DefaultResolverFactory : Factories.DefaultResolverFactory {

        #region fields

        private static readonly ICreator<IDefaultResolver, VersionMatchingStrategies, SearchOption> _resolverCreator =
            Factory.Instance.Creator.Create<IDefaultResolver, VersionMatchingStrategies, SearchOption>((in1, in2) => new DefaultResolver(in1, in2));

        private static readonly ICreator<IDefaultResolverData, FileInfo> _dataCreator =
            Factory.Instance.Creator.Create<IDefaultResolverData, FileInfo>((in1) => new DefaultResolverData(in1));

        #endregion

        #region methods

        public override void Create(out IDefaultResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, SearchOption searchOption)
            => _resolverCreator.Create(out obj, assemblyMatchingStrategy, searchOption);

        public override Boolean TryCreate(out IDefaultResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, SearchOption searchOption)
            => _resolverCreator.TryCreate(out obj, assemblyMatchingStrategy, searchOption);

        public override Boolean TryCreate(out IDefaultResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, SearchOption searchOption, out Exception ex)
            => _resolverCreator.TryCreate(out obj, assemblyMatchingStrategy, searchOption, out ex);

        public override void Create(out IDefaultResolverData obj, FileInfo file)
            => _dataCreator.Create(out obj, file);

        public override Boolean TryCreate(out IDefaultResolverData obj, FileInfo file)
            => _dataCreator.TryCreate(out obj, file);

        public override Boolean TryCreate(out IDefaultResolverData obj, FileInfo file, out Exception ex)
            => _dataCreator.TryCreate(out obj, file, out ex);

        #endregion

    }

}
