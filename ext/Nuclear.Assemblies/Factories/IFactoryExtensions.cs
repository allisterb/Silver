using Nuclear.Creation;

namespace Nuclear.Assemblies.Factories {

    /// <summary>
    /// Extends the functionality of <see cref="IFactory"/>.
    /// </summary>
    public static class IFactoryExtensions {

        /// <summary>
        /// Returns a new instance of type <see cref="DefaultResolverFactory"/>.
        /// </summary>
        /// <param name="this">The extended <see cref="IFactory"/> instance.</param>
        /// <returns>A new instance of type <see cref="DefaultResolverFactory"/>.</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static DefaultResolverFactory DefaultResolver(this IFactory @this) => new Internal.DefaultResolverFactory();
#pragma warning restore IDE0060 // Remove unused parameter

        /// <summary>
        /// Returns a new instance of type <see cref="NugetResolverFactory"/>.
        /// </summary>
        /// <param name="this">The extended <see cref="IFactory"/> instance.</param>
        /// <returns>A new instance of type <see cref="NugetResolverFactory"/>.</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public static NugetResolverFactory NugetResolver(this IFactory @this) => new Internal.NugetResolverFactory();
#pragma warning restore IDE0060 // Remove unused parameter

    }

}
