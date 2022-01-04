namespace Nuclear.Assemblies.Runtimes {

    /// <summary>
    /// Defines currently supported implementations of the .NET platform.
    /// </summary>
    public enum FrameworkIdentifiers {

        /// <summary>
        /// Unknown TFM placeholder.
        /// </summary>
        Unsupported,

        /// <summary>
        /// .NET Standard Api framework.
        /// </summary>
        NETStandard,

        /// <summary>
        /// .NET Desktop framework.
        /// </summary>
        NETFramework,

        /// <summary>
        /// .NET Core framework.
        /// </summary>
        NETCoreApp,

    }
}
