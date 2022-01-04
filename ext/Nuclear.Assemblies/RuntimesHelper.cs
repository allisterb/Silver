using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

using Nuclear.Assemblies.Extensions;
using Nuclear.Assemblies.Runtimes;
using Nuclear.Extensions;

namespace Nuclear.Assemblies {

    /// <summary>
    /// Helper class providing methods for analyzing and handling runtime information data.
    /// </summary>
    public static class RuntimesHelper {

        #region properties

        internal static IDictionary<RuntimeInfo, Version> NetStandardVersions { get; }
            = new Dictionary<RuntimeInfo, Version>() {

                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(1, 1)), null },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(2, 0)), null },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(3, 5)), null },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(4, 0)), null },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(4, 5)), new Version(1, 1) },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(4, 5, 1)), new Version(1, 2) },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(4, 5, 2)), new Version(1, 2) },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(4, 6)), new Version(1, 3) },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(4, 6, 1)), new Version(2, 0) },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(4, 6, 2)), new Version(2, 0) },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(4, 7)), new Version(2, 0) },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(4, 7, 1)), new Version(2, 0) },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(4, 7, 2)), new Version(2, 0) },
                { new RuntimeInfo(FrameworkIdentifiers.NETFramework, new Version(4, 8)), new Version(2, 0) },

                { new RuntimeInfo(FrameworkIdentifiers.NETCoreApp, new Version(1, 0)), new Version(1, 6) },
                { new RuntimeInfo(FrameworkIdentifiers.NETCoreApp, new Version(1, 1)), new Version(1, 6) },
                { new RuntimeInfo(FrameworkIdentifiers.NETCoreApp, new Version(2, 0)), new Version(2, 0) },
                { new RuntimeInfo(FrameworkIdentifiers.NETCoreApp, new Version(2, 1)), new Version(2, 0) },
                { new RuntimeInfo(FrameworkIdentifiers.NETCoreApp, new Version(2, 2)), new Version(2, 0) },
                { new RuntimeInfo(FrameworkIdentifiers.NETCoreApp, new Version(3, 0)), new Version(2, 1) },
                { new RuntimeInfo(FrameworkIdentifiers.NETCoreApp, new Version(3, 1)), new Version(2, 1) },
                { new RuntimeInfo(FrameworkIdentifiers.NETCoreApp, new Version(5, 0)), new Version(2, 1) },
                { new RuntimeInfo(FrameworkIdentifiers.NETCoreApp, new Version(6, 0)), new Version(2, 1) },

                { new RuntimeInfo(FrameworkIdentifiers.NETStandard, new Version(1, 0)), null },
                { new RuntimeInfo(FrameworkIdentifiers.NETStandard, new Version(1, 1)), null },
                { new RuntimeInfo(FrameworkIdentifiers.NETStandard, new Version(1, 2)), null },
                { new RuntimeInfo(FrameworkIdentifiers.NETStandard, new Version(1, 3)), null },
                { new RuntimeInfo(FrameworkIdentifiers.NETStandard, new Version(1, 4)), null },
                { new RuntimeInfo(FrameworkIdentifiers.NETStandard, new Version(1, 5)), null },
                { new RuntimeInfo(FrameworkIdentifiers.NETStandard, new Version(1, 6)), null },
                { new RuntimeInfo(FrameworkIdentifiers.NETStandard, new Version(2, 0)), null },
                { new RuntimeInfo(FrameworkIdentifiers.NETStandard, new Version(2, 1)), null },

            };

        #endregion

        #region current runtime

        /// <summary>
        /// Gets an instance of the currently executed runtime version.
        /// </summary>
        /// <param name="runtime">The current runtime version.</param>
        /// <returns>True if the runtime version could be retrieved.</returns>
        public static Boolean TryGetCurrentRuntime(out RuntimeInfo runtime) {
            runtime = null;

            Assembly entryAsm = Assembly.GetEntryAssembly();
            TargetFrameworkAttribute attr = entryAsm.GetCustomAttribute<TargetFrameworkAttribute>();

            return attr != null && TryParseFullName(attr.FrameworkName, out runtime) && runtime.Framework != FrameworkIdentifiers.Unsupported;
        }

        #endregion

        #region target framework moniker parsing

        /// <summary>
        /// Converts a target framework moniker into a <see cref="RuntimeInfo"/> instance.
        /// A return value indicates if the conversion operation was successful.
        /// </summary>
        /// <param name="tfm"></param>
        /// <param name="runtimeInfo">The created <see cref="RuntimeInfo"/> instance.</param>
        /// <returns>True if <paramref name="runtimeInfo"/> could be created.</returns>
        public static Boolean TryParseTFM(String tfm, out RuntimeInfo runtimeInfo) {
            runtimeInfo = null;

            if(!String.IsNullOrWhiteSpace(tfm) && SplitTFM(tfm.ToLower(), out FrameworkIdentifiers identifier, out Version version)) {
                try {
                    runtimeInfo = new RuntimeInfo(identifier, version);

                } catch {
                    return false;
                }
            }

            return runtimeInfo != null && runtimeInfo.Framework != FrameworkIdentifiers.Unsupported;
        }

        internal static Boolean SplitTFM(String tfm, out FrameworkIdentifiers identifier, out Version version) {
            identifier = FrameworkIdentifiers.Unsupported;
            version = null;

            if(!String.IsNullOrWhiteSpace(tfm)) {
                String _tfm = tfm.Trim().ToLower();
                String _identifier = String.Concat(_tfm.TakeWhile(Char.IsLetter));
                String _version = _tfm.Substring(_identifier.Length);

                identifier = _identifier switch {
                    "netstandard" => FrameworkIdentifiers.NETStandard,
                    "netcoreapp" => FrameworkIdentifiers.NETCoreApp,
                    "net" => _version.Contains('.') ? FrameworkIdentifiers.NETCoreApp : FrameworkIdentifiers.NETFramework,
                    _ => FrameworkIdentifiers.Unsupported,
                };

                if(identifier != FrameworkIdentifiers.Unsupported && _version.All(c => Char.IsDigit(c) || c == '.')) {
                    if(!Version.TryParse(_version, out version)) {
                        version = new Version(String.Join(".", _version.Select(c => c.ToString())));
                    }
                }
            }

            return Enum.IsDefined(typeof(FrameworkIdentifiers), identifier) && version != null;
        }

        #endregion

        #region assembly full name parsing

        /// <summary>
        /// Converts a runtime name into a <see cref="RuntimeInfo"/> instance.
        /// A return value indicates if the conversion operation was successful.
        /// </summary>
        /// <param name="fullName">The full runtime name.</param>
        /// <param name="runtimeInfo">The created <see cref="RuntimeInfo"/> instance.</param>
        /// <returns>True if <paramref name="runtimeInfo"/> could be created.</returns>
        public static Boolean TryParseFullName(String fullName, out RuntimeInfo runtimeInfo) {
            runtimeInfo = null;

            ParseParts(fullName, out FrameworkIdentifiers framework, out Version version);

            try {
                runtimeInfo = new RuntimeInfo(framework, version);

            } catch { /* Don't worry about exceptions here */ }

            return runtimeInfo != null && runtimeInfo.Framework != FrameworkIdentifiers.Unsupported;
        }

        internal static void ParseParts(String fullName, out FrameworkIdentifiers framework, out Version version) {
            framework = FrameworkIdentifiers.Unsupported;
            version = null;

            List<String> parts = fullName.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if(parts.TryTake(part => part.Trim().StartsWith("Version=v"), out String versionPart)) {
                String tmp = versionPart.Trim().TrimStartOnce("Version=v");

                try {
                    version = new Version(tmp);

                } catch { /* Don't worry about exceptions here */ }
            }

            Predicate<String> match = part => {
                try {
                    Enum.Parse(typeof(FrameworkIdentifiers), part.Trim().TrimStartOnce('.'), true);
                    return true;

                } catch { return false; }
            };

            if(parts.TryTake(match, out String frameworkPart)) {
                framework = (FrameworkIdentifiers) Enum.Parse(typeof(FrameworkIdentifiers), frameworkPart.Trim().TrimStartOnce('.'), true);
            }
        }

        #endregion

        #region matching methods

        /// <summary>
        /// Gets the <see cref="Version"/> of .NETStandard that is implemented by <paramref name="runtime"/>.
        /// A return value indicates if a version could be retrieved.
        /// </summary>
        /// <param name="runtime">The runtime to check.</param>
        /// <param name="netStandardVersion">The <see cref="Version"/> of .NETStandard or null if not implemented.</param>
        /// <returns>True if <paramref name="runtime"/> implements a <see cref="Version"/> of .NETStandard.</returns>
        public static Boolean TryGetStandardVersion(RuntimeInfo runtime, out Version netStandardVersion) {
            netStandardVersion = null;

            if(runtime != null && NetStandardVersions.TryGetValue(runtime, out Version version)) {
                netStandardVersion = version;
            }

            return netStandardVersion != null;
        }

        /// <summary>
        /// Gets all matching runtimes that can load an assembly targeting <paramref name="runtime"/>.
        /// A return value indicates if a runtime could be retrieved.
        /// </summary>
        /// <param name="runtime">The runtime to check.</param>
        /// <param name="runtimes">A collection of matching runtimes.</param>
        /// <returns>True if any matching runtimes were found.</returns>
        public static Boolean TryGetMatchingRuntimes(RuntimeInfo runtime, out IEnumerable<RuntimeInfo> runtimes) {
            runtimes = Enumerable.Empty<RuntimeInfo>();

            if(runtime != null) {
                runtimes = runtime.Framework switch {
                    FrameworkIdentifiers.NETStandard => NetStandardVersions.Keys.Where(key => TryGetStandardVersion(key, out Version version) && version >= runtime.Version),
                    _ => NetStandardVersions.Keys.Where(key => key.Framework == runtime.Framework && key.Version >= runtime.Version),
                };
            }

            return runtimes.Count() > 0;
        }

        /// <summary>
        /// Gets the lowest matching runtime of all supported frameworks that can load an assembly targeting <paramref name="runtime"/>.
        /// A return value indicates if a runtime could be retrieved.
        /// </summary>
        /// <param name="runtime">The runtime to check.</param>
        /// <param name="runtimes">A collection of matching runtimes.</param>
        /// <returns>True if any matching runtimes were found.</returns>
        public static Boolean TryGetLowestMatchingRuntimes(RuntimeInfo runtime, out IEnumerable<RuntimeInfo> runtimes) {
            runtimes = Enumerable.Empty<RuntimeInfo>();

            if(TryGetMatchingRuntimes(runtime, out IEnumerable<RuntimeInfo> matchingRuntimes)) {
                List<RuntimeInfo> _runtimes = new List<RuntimeInfo>();

                matchingRuntimes
                    .Select(_runtime => _runtime.Framework)
                    .Distinct()
                    .Foreach(tfm => _runtimes.Add(new RuntimeInfo(tfm, matchingRuntimes.Where(_runtime => _runtime.Framework == tfm).Select(_runtime => _runtime.Version).MinimumT())));

                runtimes = _runtimes;
            }

            return runtimes.Count() > 0;
        }

        /// <summary>
        /// Gets all supported runtimes that can be loaded by an assembly targeting <paramref name="runtime"/>.
        /// A return value indicates if a runtime could be retrieved.
        /// </summary>
        /// <param name="runtime">The runtime to check.</param>
        /// <param name="runtimes">A collection of matching runtimes.</param>
        /// <returns>True if any matching runtimes were found.</returns>
        public static Boolean TryGetLoadableRuntimes(RuntimeInfo runtime, out IEnumerable<RuntimeInfo> runtimes) {
            runtimes = Enumerable.Empty<RuntimeInfo>();

            if(runtime != null) {
                runtimes = NetStandardVersions.Keys.Where(key => (key.Framework == runtime.Framework && key.Version <= runtime.Version)
                    || (TryGetStandardVersion(runtime, out Version standardVersion) && key.Framework == FrameworkIdentifiers.NETStandard && key.Version <= standardVersion));
            }

            return runtimes.Count() > 0;
        }

        /// <summary>
        /// Gets all supported runtimes that can be loaded by an assembly targeting <paramref name="runtime"/>.
        /// A return value indicates if a runtime could be retrieved.
        /// Returned runtimes are sorted by available feature sets.
        /// </summary>
        /// <param name="runtime">The runtime to check.</param>
        /// <param name="sortDescending">Sorts matching runtimes descending if true and ascending if false.</param>
        /// <param name="runtimes">A collection of matching runtimes.</param>
        /// <returns>True if any matching runtimes were found.</returns>
        public static Boolean TryGetLoadableRuntimes(RuntimeInfo runtime, Boolean sortDescending, out List<RuntimeInfo> runtimes) {
            runtimes = new List<RuntimeInfo>();

            if(TryGetLoadableRuntimes(runtime, out IEnumerable<RuntimeInfo> unsortedRuntimes)) {
                runtimes.AddRange(unsortedRuntimes);
                runtimes.Sort(new RuntimeInfoFeatureComparer().Compare);
            }

            if(sortDescending) {
                runtimes.Reverse();
            }

            return runtimes.Count() > 0;
        }

        #endregion

    }
}
