using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Nuclear.Assemblies.ResolverData;

namespace Nuclear.Assemblies.Resolvers.Internal {
    internal interface ICoreDefaultResolver {

        #region methods

        IEnumerable<IDefaultResolverData> Resolve(AssemblyName assemblyName, DirectoryInfo searchDir, SearchOption searchOption, VersionMatchingStrategies assemblyMatchingStrategy);

        #endregion

    }
}
