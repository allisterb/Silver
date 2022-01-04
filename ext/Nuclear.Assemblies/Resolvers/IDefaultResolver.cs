using System.IO;

using Nuclear.Assemblies.ResolverData;

namespace Nuclear.Assemblies.Resolvers {

    /// <summary>
    /// A resolver that searches in a directory.
    /// Directories can be the location of the calling assembly or of the entry assembly.
    /// </summary>
    public interface IDefaultResolver : IAssemblyResolver<IDefaultResolverData> {

        #region properties

        /// <summary>
        /// Gets the search strategy for directories.
        /// </summary>
        SearchOption SearchOption { get; }

        #endregion

    }

}
