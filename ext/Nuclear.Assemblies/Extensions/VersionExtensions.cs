using System;
using System.Collections.Generic;

using Nuclear.Assemblies.Resolvers;
using Nuclear.Extensions;

namespace Nuclear.Assemblies.Extensions {
    internal static class VersionExtensions {

        private static readonly IEqualityComparer<Version> _strictComparer =
            DynamicEqualityComparer.FromDelegate<Version>((x, y) => x.Major == y.Major && x.Minor == y.Minor && x.Build == y.Build, (x) => x.GetHashCode());

        private static readonly IEqualityComparer<Version> _semVerComparer =
            DynamicEqualityComparer.FromDelegate<Version>((x, y) => x.Major == y.Major && x.Minor <= y.Minor, (x) => x.GetHashCode());

        internal static Boolean Matches(this Version requested, Version found, VersionMatchingStrategies strategy) {
            if(requested == null || found == null) {
                return false;
            }

            return strategy switch {
                VersionMatchingStrategies.Strict => _strictComparer.Equals(requested, found),
                VersionMatchingStrategies.SemVer => _semVerComparer.Equals(requested, found),
                _ => false,
            };
        }
    }
}
