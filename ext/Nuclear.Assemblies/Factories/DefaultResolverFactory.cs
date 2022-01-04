using System;
using System.IO;

using Nuclear.Assemblies.ResolverData;
using Nuclear.Assemblies.Resolvers;
using Nuclear.Creation;

namespace Nuclear.Assemblies.Factories {

    /// <summary>
    /// Defines a factory to create instances of default assembly resolvers and their data objects.
    /// </summary>
    public abstract class DefaultResolverFactory :
        ICreator<IDefaultResolver, VersionMatchingStrategies, SearchOption>,
        ICreator<IDefaultResolverData, FileInfo> {

        /// <summary>
        /// Creates an instance of <see name="IDefaultResolver"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="IDefaultResolver"/>.</param>
        /// <param name="assemblyMatchingStrategy">The matching strategy for assembly versions.</param>
        /// <param name="searchOption">The search strategy for directories.</param>
        public abstract void Create(out IDefaultResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, SearchOption searchOption);

        /// <summary>
        /// Tries to create an instance of <see name="IDefaultResolver"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="IDefaultResolver"/>.</param>
        /// <param name="assemblyMatchingStrategy">The matching strategy for assembly versions.</param>
        /// <param name="searchOption">The search strategy for directories.</param>
        /// <returns>True if the object was created.</returns>
        public abstract Boolean TryCreate(out IDefaultResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, SearchOption searchOption);

        /// <summary>
        /// Tries to create an instance of <see name="IDefaultResolver"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="IDefaultResolver"/>.</param>
        /// <param name="assemblyMatchingStrategy">The matching strategy for assembly versions.</param>
        /// <param name="searchOption">The search strategy for directories.</param>
        /// <param name="ex">The caught exception.</param>
        /// <returns>True if the object was created.</returns>
        public abstract Boolean TryCreate(out IDefaultResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, SearchOption searchOption, out Exception ex);

        /// <summary>
        /// Creates an instance of <see name="IDefaultResolverData"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="IDefaultResolverData"/>.</param>
        /// <param name="file">The resolved assembly file.</param>
        public abstract void Create(out IDefaultResolverData obj, FileInfo file);

        /// <summary>
        /// Tries to create an instance of <see name="IDefaultResolverData"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="IDefaultResolverData"/>.</param>
        /// <param name="file">The resolved assembly file.</param>
        /// <returns>True if the object was created.</returns>
        public abstract Boolean TryCreate(out IDefaultResolverData obj, FileInfo file);

        /// <summary>
        /// Tries to create an instance of <see name="IDefaultResolverData"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="IDefaultResolverData"/>.</param>
        /// <param name="file">The resolved assembly file.</param>
        /// <param name="ex">The caught exception.</param>
        /// <returns>True if the object was created.</returns>
        public abstract Boolean TryCreate(out IDefaultResolverData obj, FileInfo file, out Exception ex);

    }

}
