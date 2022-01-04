
using Nuclear.Exceptions;
using Nuclear.Extensions;

namespace Nuclear.Assemblies.Resolvers.Internal {

    internal abstract class AssemblyResolver : IAssemblyResolver {

        #region properties

        public VersionMatchingStrategies AssemblyMatchingStrategy { get; }

        #endregion

        #region ctors

        internal AssemblyResolver(VersionMatchingStrategies assemblyMatchingStrategy) {
            Throw.IfNot.Enum.IsDefined<VersionMatchingStrategies>(assemblyMatchingStrategy, nameof(assemblyMatchingStrategy), $"Given strategy is not defined {assemblyMatchingStrategy.Format()}");

            AssemblyMatchingStrategy = assemblyMatchingStrategy;
        }

        #endregion

    }

}
