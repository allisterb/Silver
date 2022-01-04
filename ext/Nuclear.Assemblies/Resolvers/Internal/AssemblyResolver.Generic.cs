using System;
using System.Collections.Generic;
using System.Reflection;

using Nuclear.Assemblies.ResolverData;

namespace Nuclear.Assemblies.Resolvers.Internal {

    internal abstract class AssemblyResolver<TData> : AssemblyResolver, IAssemblyResolver<TData> where TData : IAssemblyResolverData {

        #region ctors

        internal AssemblyResolver(VersionMatchingStrategies assemblyMatchingStrategy) : base(assemblyMatchingStrategy) { }

        #endregion

        #region public methods

        public abstract Boolean TryResolve(ResolveEventArgs e, out IEnumerable<TData> data);

        public abstract Boolean TryResolve(String fullName, out IEnumerable<TData> data);

        public abstract Boolean TryResolve(AssemblyName assemblyName, out IEnumerable<TData> data);

        #endregion

    }

}
