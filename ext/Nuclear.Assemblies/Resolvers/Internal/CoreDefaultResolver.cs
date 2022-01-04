using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Nuclear.Assemblies.Extensions;
using Nuclear.Assemblies.Factories;
using Nuclear.Assemblies.ResolverData;
using Nuclear.Creation;

namespace Nuclear.Assemblies.Resolvers.Internal {
    internal class CoreDefaultResolver : ICoreDefaultResolver {

        #region fields

        private static readonly ICreator<IDefaultResolverData, FileInfo> _factory = Factory.Instance.DefaultResolver();

        #endregion

        #region methods

        public IEnumerable<IDefaultResolverData> Resolve(AssemblyName assemblyName, DirectoryInfo searchDir, SearchOption searchOption, VersionMatchingStrategies assemblyMatchingStrategy) {
            List<IDefaultResolverData> files = new List<IDefaultResolverData>();

            if(assemblyName != null && searchDir != null) {

                foreach(FileInfo file in searchDir.EnumerateFiles($"{assemblyName.Name}.dll", searchOption)) {

                    if(_factory.TryCreate(out IDefaultResolverData data, file)
                        && assemblyName.Name == data.AssemblyName.Name
                        && assemblyName.Version.Matches(data.AssemblyName.Version, assemblyMatchingStrategy)
                        && AssemblyHelper.ValidateArchitecture(data.AssemblyName)) {

                        files.Add(data);
                    }
                }
            }

            return files;
        }

        #endregion

    }
}
