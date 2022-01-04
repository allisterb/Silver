using System;
using System.IO;
using System.Reflection;

using Nuclear.Assemblies.Resolvers;

namespace Nuclear.Assemblies.ResolverData {

    /// <summary>
    /// Defines the assembly information that was found by an <see cref="IAssemblyResolver{TData}"/>.
    /// </summary>
    public interface IAssemblyResolverData {

        #region properties

        /// <summary>
        /// Gets the resolved assembly.
        /// </summary>
        FileInfo File { get; }

        /// <summary>
        /// Gets the <see cref="System.Reflection.AssemblyName"/> of the assembly.
        /// </summary>
        AssemblyName AssemblyName { get; }

        /// <summary>
        /// Gets if the given <see cref="FileInfo"/> is valid.
        /// </summary>
        Boolean IsValid { get; }

        #endregion

    }

}
