using System;

namespace Nuclear.Assemblies {
    internal static class EnvironmentHelper {

        #region variables

        public static Boolean TryGetVariable(String variable, out String value) => TryGetVariable(variable, EnvironmentVariableTarget.Process, out value);

        public static Boolean TryGetVariable(String variable, EnvironmentVariableTarget target, out String value) {
            value = null;

            try {
                value = Environment.GetEnvironmentVariable(variable, target);

            } catch { /* Don't worry about exceptions here */ }

            return value != null;
        }

        public static Boolean TryGetVariableForAllTargets(String variable, out String value) {
            value = null;

            foreach(Object target in Enum.GetValues(typeof(EnvironmentVariableTarget))) {
                if(TryGetVariable(variable, (EnvironmentVariableTarget) target, out value)) {
                    break;
                }
            }

            return value != null;
        }

        #endregion

    }
}
