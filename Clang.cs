namespace ClangSharp {
    class Clang {

        public static bool CrashRecovery {
            set { Interop.clang_toggleCrashRecovery(value ? 1u : 0u); }
        }

        private static string _version;
        public static string Version {
            get { return _version ?? (_version = Interop.clang_getClangVersion().ManagedString); }
        }
    }
}
