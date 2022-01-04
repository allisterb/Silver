using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Nuclear.Assemblies.ResolverData;
using Nuclear.Exceptions;
using Nuclear.Extensions;

namespace Nuclear.Assemblies.Resolvers.Internal {

    internal class DefaultResolver : AssemblyResolver<IDefaultResolverData>, IDefaultResolver {

        #region properties

        public SearchOption SearchOption { get; }

        internal ICoreDefaultResolver CoreResolver { get; set; } = new CoreDefaultResolver();

        #endregion

        #region ctors

        internal DefaultResolver(VersionMatchingStrategies assemblyMatchingStrategy, SearchOption searchOption) : base(assemblyMatchingStrategy) {
            Throw.IfNot.Enum.IsDefined<SearchOption>(searchOption, nameof(searchOption), $"Given search option is not defined {searchOption.Format()}");

            SearchOption = searchOption;
        }

        #endregion

        #region public methods

        public override Boolean TryResolve(ResolveEventArgs e, out IEnumerable<IDefaultResolverData> data) {
            data = Enumerable.Empty<IDefaultResolverData>();

            if(AssemblyHelper.TryGetAssemblyName(e, out AssemblyName assemblyName)) {
                if(e.RequestingAssembly == null) {
                    data = CoreResolver.Resolve(assemblyName, new FileInfo(Assembly.GetEntryAssembly().Location).Directory, SearchOption, AssemblyMatchingStrategy);
                } else {
                    data = CoreResolver.Resolve(assemblyName, new FileInfo(e.RequestingAssembly.Location).Directory, SearchOption, AssemblyMatchingStrategy);
                }
            }

            return data != null && data.Count() > 0;
        }

        public override Boolean TryResolve(String fullName, out IEnumerable<IDefaultResolverData> data) {
            data = Enumerable.Empty<IDefaultResolverData>();

            return AssemblyHelper.TryGetAssemblyName(fullName, out AssemblyName assemblyName) && TryResolve(assemblyName, out data);
        }

        public override Boolean TryResolve(AssemblyName assemblyName, out IEnumerable<IDefaultResolverData> data) {
            data = Enumerable.Empty<IDefaultResolverData>();

            if(assemblyName != null) {
                data = CoreResolver.Resolve(assemblyName, new FileInfo(Assembly.GetEntryAssembly().Location).Directory, SearchOption, AssemblyMatchingStrategy);
            }

            return data != null && data.Count() > 0;
        }

        #endregion

    }

}
