using System;
using System.Collections.Generic;
using System.IO;

using Nuclear.Assemblies.ResolverData;
using Nuclear.Assemblies.Resolvers;
using Nuclear.Creation;

namespace Nuclear.Assemblies.Factories {

    /// <summary>
    /// Defines a factory to create instances of nuget assembly resolvers and their data objects.
    /// </summary>
    public abstract class NugetResolverFactory :
        ICreator<INugetResolver, VersionMatchingStrategies, VersionMatchingStrategies>,
        ICreator<INugetResolver, VersionMatchingStrategies, VersionMatchingStrategies, IEnumerable<DirectoryInfo>>,
        ICreator<INugetResolverData, FileInfo> {

        /// <summary>
        /// Creates an instance of <see name="INugetResolver"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="INugetResolver"/>.</param>
        /// <param name="assemblyMatchingStrategy">The matching strategy for assembly versions.</param>
        /// <param name="packageMatchingStrategy">The matching strategy for package versions.</param>
        public abstract void Create(out INugetResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy);

        /// <summary>
        /// Tries to create an instance of <see name="INugetResolver"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="INugetResolver"/>.</param>
        /// <param name="assemblyMatchingStrategy">The matching strategy for assembly versions.</param>
        /// <param name="packageMatchingStrategy">The matching strategy for package versions.</param>
        /// <returns>True if the object was created.</returns>
        public abstract Boolean TryCreate(out INugetResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy);

        /// <summary>
        /// Tries to create an instance of <see name="INugetResolver"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="INugetResolver"/>.</param>
        /// <param name="assemblyMatchingStrategy">The matching strategy for assembly versions.</param>
        /// <param name="packageMatchingStrategy">The matching strategy for package versions.</param>
        /// <param name="ex">The caught exception.</param>
        /// <returns>True if the object was created.</returns>
        public abstract Boolean TryCreate(out INugetResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy, out Exception ex);

        /// <summary>
        /// Creates an instance of <see name="INugetResolver"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="INugetResolver"/>.</param>
        /// <param name="assemblyMatchingStrategy">The matching strategy for assembly versions.</param>
        /// <param name="packageMatchingStrategy">The matching strategy for package versions.</param>
        /// <param name="caches">The used NuGet package cache directories.</param>
        public abstract void Create(out INugetResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy, IEnumerable<DirectoryInfo> caches);

        /// <summary>
        /// Tries to create an instance of <see name="INugetResolver"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="INugetResolver"/>.</param>
        /// <param name="assemblyMatchingStrategy">The matching strategy for assembly versions.</param>
        /// <param name="packageMatchingStrategy">The matching strategy for package versions.</param>
        /// <param name="caches">The used NuGet package cache directories.</param>
        /// <returns>True if the object was created.</returns>
        public abstract Boolean TryCreate(out INugetResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy, IEnumerable<DirectoryInfo> caches);

        /// <summary>
        /// Tries to create an instance of <see name="INugetResolver"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="INugetResolver"/>.</param>
        /// <param name="assemblyMatchingStrategy">The matching strategy for assembly versions.</param>
        /// <param name="packageMatchingStrategy">The matching strategy for package versions.</param>
        /// <param name="caches">The used NuGet package cache directories.</param>
        /// <param name="ex">The caught exception.</param>
        /// <returns>True if the object was created.</returns>
        public abstract Boolean TryCreate(out INugetResolver obj, VersionMatchingStrategies assemblyMatchingStrategy, VersionMatchingStrategies packageMatchingStrategy, IEnumerable<DirectoryInfo> caches, out Exception ex);

        /// <summary>
        /// Creates an instance of <see name="INugetResolverData"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="INugetResolverData"/>.</param>
        /// <param name="file">The resolved assembly file.</param>
        public abstract void Create(out INugetResolverData obj, FileInfo file);

        /// <summary>
        /// Tries to create an instance of <see name="INugetResolverData"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="INugetResolverData"/>.</param>
        /// <param name="file">The resolved assembly file.</param>
        /// <returns>True if the object was created.</returns>
        public abstract Boolean TryCreate(out INugetResolverData obj, FileInfo file);

        /// <summary>
        /// Tries to create an instance of <see name="INugetResolverData"/> and returns it via the out parameter <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The created instance of type <see name="INugetResolverData"/>.</param>
        /// <param name="file">The resolved assembly file.</param>
        /// <param name="ex">The caught exception.</param>
        /// <returns>True if the object was created.</returns>
        public abstract Boolean TryCreate(out INugetResolverData obj, FileInfo file, out Exception ex);

    }

}
