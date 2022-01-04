using System;
using Nuclear.Exceptions;

namespace Nuclear.Assemblies.Runtimes {

    /// <summary>
    /// Represents target framework data of a runtime or library.
    /// </summary>
    public class RuntimeInfo : IEquatable<RuntimeInfo> {

        #region properties

        /// <summary>
        /// Gets the targeted framework of the runtime.
        /// </summary>
        public FrameworkIdentifiers Framework { get; }

        /// <summary>
        /// Gets the framework version of the runtime.
        /// </summary>
        public Version Version { get; }

        #endregion

        #region ctors

        /// <summary>
        /// Creates a new instance of <see cref="RuntimeInfo"/>.
        /// </summary>
        /// <param name="framework">The targeted framework.</param>
        /// <param name="version">The framework version.</param>
        public RuntimeInfo(FrameworkIdentifiers framework, Version version) {
            Throw.IfNot.Enum.IsDefined<FrameworkIdentifiers>(framework, nameof(framework));
            Throw.If.Object.IsNull(version, nameof(version));

            Framework = framework;
            Version = version;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override Boolean Equals(Object obj) {
            if(obj != null && obj is RuntimeInfo other) {
                return Equals(other);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public Boolean Equals(RuntimeInfo other) {
            if(other == null) { return false; }

            return Framework == other.Framework && Version == other.Version;
        }

        /// <summary>
        /// Gets a hash value of the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => Framework.GetHashCode() + Version.GetHashCode();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => $"{Framework} {Version}";

        #endregion

    }
}
