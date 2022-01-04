namespace Nuclear.Assemblies.Resolvers {

    /// <summary>
    /// Defines a range of version matching strategies to resolve assemblies.
    /// </summary>
    public enum VersionMatchingStrategies {

        /// <summary>
        /// Versions must match exactly.
        /// </summary>
        Strict = 0,

        /// <summary>
        /// Version matching is done according to semver.
        /// </summary>
        SemVer = 1,
    }
}
