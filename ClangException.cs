using System;

namespace ClangSharp {
    public class ClangException : Exception {
        public ClangException()
            : base("An error occurred in libclang") {
        }

        public ClangException(string message)
            : base(message) {
        }

        public ClangException(string message, params object[] args)
            : base(string.Format(message, args)) {
        }
    }
}
