using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

using Nuclear.Assemblies.Runtimes;

namespace Nuclear.Assemblies {

    /// <summary>
    /// Provides helper methods to handle assembly related tasks.
    /// </summary>
    public static class AssemblyHelper {

        #region fields

        internal static ProcessorArchitecture[] _validArchitectures =
            new ProcessorArchitecture[] { ProcessorArchitecture.MSIL, Environment.Is64BitProcess ? ProcessorArchitecture.Amd64 : ProcessorArchitecture.X86 };

        #endregion

        #region properties

        /// <summary>
        /// Gets a list of valid file extensions for .NET assemblies.
        /// </summary>
        public static String[] AssemblyFileExtensions { get; } = new String[] { "dll", "exe" };

        #endregion

        #region loading

        /// <summary>
        /// Loads an assembly from disk.
        /// </summary>
        /// <param name="file">The file on disk.</param>
        /// <param name="assembly">The loaded assembly.</param>
        /// <returns>True if the assembly could be loaded.</returns>
        public static Boolean TryLoadFile(FileInfo file, out Assembly assembly) {
            assembly = null;

            if(file != null && file.Exists) {
                try {
                    assembly = Assembly.LoadFile(file.FullName);

                } catch { /* Don't worry about exceptions here */ }
            }

            return assembly != null;
        }

        /// <summary>
        /// Loads an assembly from disk.
        /// </summary>
        /// <param name="file">The file on disk.</param>
        /// <param name="assembly">The loaded assembly.</param>
        /// <returns>True if the assembly could be loaded.</returns>
        public static Boolean TryLoadFrom(FileInfo file, out Assembly assembly) {
            assembly = null;

            if(file != null && file.Exists) {
                try {
                    assembly = Assembly.LoadFrom(file.FullName);

                } catch { /* Don't worry about exceptions here */ }
            }

            return assembly != null;
        }

        /// <summary>
        /// Loads an assembly from disk.
        /// </summary>
        /// <param name="file">The file on disk.</param>
        /// <param name="assembly">The loaded assembly.</param>
        /// <returns>True if the assembly could be loaded.</returns>
        public static Boolean TryUnsafeLoadFrom(FileInfo file, out Assembly assembly) {
            assembly = null;

            if(file != null && file.Exists) {
                try {
                    assembly = Assembly.UnsafeLoadFrom(file.FullName);

                } catch { /* Don't worry about exceptions here */ }
            }

            return assembly != null;
        }

        #endregion

        #region assembly name

        /// <summary>
        /// Gets the <see cref="AssemblyName"/> by parsing <paramref name="e"/>.
        /// </summary>
        /// <param name="e">The event arguments used to parse.</param>
        /// <param name="assemblyName">The resulting <see cref="AssemblyName"/>.</param>
        /// <returns>True if the <see cref="AssemblyName"/> could be retrieved.</returns>
        public static Boolean TryGetAssemblyName(ResolveEventArgs e, out AssemblyName assemblyName) {
            assemblyName = null;

            try {
                assemblyName = new AssemblyName(e.Name);

            } catch { return false; }

            return assemblyName != null;
        }

        /// <summary>
        /// Gets the <see cref="AssemblyName"/> by parsing <paramref name="fullName"/>.
        /// </summary>
        /// <param name="fullName">The full assembly name.</param>
        /// <param name="assemblyName">The resulting <see cref="AssemblyName"/>.</param>
        /// <returns>True if the <see cref="AssemblyName"/> could be retrieved.</returns>
        public static Boolean TryGetAssemblyName(String fullName, out AssemblyName assemblyName) {
            assemblyName = null;

            try {
                assemblyName = new AssemblyName(fullName);

            } catch { return false; }

            return assemblyName != null;
        }

        /// <summary>
        /// Gets the <see cref="AssemblyName"/> from an assembly file on disk.
        /// </summary>
        /// <param name="file">The assembly file on disk.</param>
        /// <param name="assemblyName">The resulting <see cref="AssemblyName"/>.</param>
        /// <returns>True if the <see cref="AssemblyName"/> could be retrieved.</returns>
        public static Boolean TryGetAssemblyName(FileInfo file, out AssemblyName assemblyName) {
            assemblyName = null;

            try {
                assemblyName = AssemblyName.GetAssemblyName(file.FullName);

            } catch { return false; }

            return assemblyName != null;
        }

        #endregion

        #region runtime info

        /// <summary>
        /// Gets the target runtime of a loaded <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">The loaded assembly.</param>
        /// <param name="runtime">The target runtime of <paramref name="assembly"/>.</param>
        /// <returns>True if the target runtime could be retrieved.</returns>
        public static Boolean TryGetRuntime(Assembly assembly, out RuntimeInfo runtime) {
            runtime = null;

            if(assembly != null) {
                TargetFrameworkAttribute attr = assembly.GetCustomAttribute<TargetFrameworkAttribute>();

                if(attr != null) {
                    RuntimesHelper.TryParseFullName(attr.FrameworkName, out runtime);
                }
            }

            return runtime != null && runtime.Framework != FrameworkIdentifiers.Unsupported && runtime.Version != new Version();
        }

        #endregion

        #region validation

        /// <summary>
        /// Validates the equality of two assembly names.
        /// </summary>
        /// <param name="lhs">The first <see cref="AssemblyName"/>.</param>
        /// <param name="rhs">The second <see cref="AssemblyName"/>.</param>
        /// <returns>True if <paramref name="lhs"/> matches <paramref name="rhs"/>.</returns>
        public static Boolean ValidateByName(AssemblyName lhs, AssemblyName rhs) => lhs != null && rhs != null && lhs.FullName == rhs.FullName;

        /// <summary>
        /// Validates the <see cref="ProcessorArchitecture"/> of an <see cref="AssemblyName"/> against the current process architecture.
        /// </summary>
        /// <param name="asmName">The <see cref="AssemblyName"/> to validate.</param>
        /// <returns>True if <paramref name="asmName"/> matches the process.</returns>
        public static Boolean ValidateArchitecture(AssemblyName asmName) => _validArchitectures.Contains(asmName.ProcessorArchitecture);

        #endregion

    }
}
