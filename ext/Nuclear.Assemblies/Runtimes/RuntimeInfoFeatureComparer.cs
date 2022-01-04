using System;
using System.Collections.Generic;

using Nuclear.Extensions;

namespace Nuclear.Assemblies.Runtimes {

    /// <summary>
    /// Implementation of <see cref="IComparer{RuntimeInfo}"/> that compares two instances of <see cref="RuntimeInfo"/>
    ///   considering the supported feature set.
    /// </summary>
    public class RuntimeInfoFeatureComparer : IComparer<RuntimeInfo> {

        /// <summary>
        /// Compares two instances of type <see cref="RuntimeInfo"/> and returns a value indicating whether one is less than,
        ///     equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first <see cref="RuntimeInfo"/> to compare.</param>
        /// <param name="y">The second <see cref="RuntimeInfo"/> to compare.</param>
        /// <returns>A signed integer that indicates the relative values of x and y, as shown in the following table.
        ///     Value Meaning Less than zero x is less than y.
        ///     Zero x equals y.
        ///     Greater than zero x is greater than y.</returns>
        public Int32 Compare(RuntimeInfo x, RuntimeInfo y) {
            if(x == null) {
                return y == null ? 0 : -Compare(y, x);

            } 
            
            if(y == null) {
                return 1;
            }

            Int32 tfResult = new FrameworkIdentifiersComparer().Compare(x.Framework, y.Framework);

            if(tfResult != 0) {
                if(x.Framework == FrameworkIdentifiers.Unsupported || y.Framework == FrameworkIdentifiers.Unsupported) {
                    return tfResult;
                }

                if(x.Framework == FrameworkIdentifiers.NETStandard) {
                    return -Compare(y, x);

                } 
                
                if(y.Framework == FrameworkIdentifiers.NETStandard) {
                    return RuntimesHelper.TryGetStandardVersion(x, out Version x_netStandardVersion) && x_netStandardVersion.IsGreaterThanOrEqual(y.Version) ? 1 : -1;
                }
            }

            return x.Version.CompareTo(y.Version);
        }

    }

}
