using System;

namespace ClangSharp {
    public class Token : IDisposable {
        internal Interop.Token Native { get; private set; }
        private readonly TranslationUnit _parent;

        internal Token(Interop.Token native, TranslationUnit parent) {
            Native = native;
            _parent = parent;
        }

        public TokenKind Kind {
            get { return Interop.clang_getTokenKind(Native); }
        }


        public string Spelling {
            get { return Interop.clang_getTokenSpelling(_parent.Native, Native).ManagedString; }
        }

        public SourceLocation TokenLocation {
            get { return new SourceLocation(Interop.clang_getTokenLocation(_parent.Native, Native)); }
        }

        public SourceRange TokenExtend {
            get { return new SourceRange(Interop.clang_getTokenExtend(_parent.Native, Native)); }
        }

        public void Dispose() {
            unsafe {
                var native = Native;
                Interop.clang_disposeTokens(_parent.Native, (IntPtr)(&native), 1);
            }
        }
    }
}
