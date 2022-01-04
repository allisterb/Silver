using System;
using System.Collections.Generic;
using System.Reflection;

using Nuclear.Assemblies.ResolverData;

namespace Nuclear.Assemblies.Resolvers {

    /// <summary>
    /// Defines an assembly resolver.
    /// </summary>
    /// <typeparam name="TData">The type of the reslver data, must inherit from <see cref="IAssemblyResolverData"/>.</typeparam>
    public interface IAssemblyResolver<TData> : IAssemblyResolver where TData : IAssemblyResolverData {

        #region methods

        /// <summary>
        /// Resolves a reference assembly by <see cref="ResolveEventArgs"/>.
        /// A return value indicates if the resolving operation was successful.
        /// </summary>
        /// <param name="e">The given <see cref="ResolveEventArgs"/>.</param>
        /// <param name="data">The resolved data.</param>
        /// <returns>True if a file could be found.</returns>
        Boolean TryResolve(ResolveEventArgs e, out IEnumerable<TData> data);

        /// <summary>
        /// Resolves a reference assembly by the full name.
        /// A return value indicates if the resolving operation was successful.
        /// </summary>
        /// <param name="fullName">The full name of the assembly.</param>
        /// <param name="data">The resolved data.</param>
        /// <returns>True if a file could be found.</returns>
        Boolean TryResolve(String fullName, out IEnumerable<TData> data);

        /// <summary>
        /// Resolves a reference assembly by an <see cref="AssemblyName"/>.
        /// A return value indicates if the resolving operation was successful.
        /// </summary>
        /// <param name="assemblyName">The <see cref="AssemblyName"/> of the assembly.</param>
        /// <param name="data">The resolved data.</param>
        /// <returns>True if a file could be found.</returns>
        Boolean TryResolve(AssemblyName assemblyName, out IEnumerable<TData> data);

        #endregion

    }
}
