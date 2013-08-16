namespace ClangSharp {
    public static class Clang {

        /// <summary>
        /// Enable/disable crash recovery.
        /// </summary>
        public static bool CrashRecovery {
            set { Interop.clang_toggleCrashRecovery(value ? 1u : 0u); }
        }

        private static string _version;

        /// <summary>
        /// A version string, suitable for showing to a user, but not intended to be parsed (the format is not guaranteed to be stable).
        /// </summary>
        public static string Version {
            get { return _version ?? (_version = Interop.clang_getClangVersion().ManagedString); }
        }
    }
}