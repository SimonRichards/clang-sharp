using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClangSharp;

namespace ClangSharp {
    public class SourceRange {
        internal Interop.SourceRange Native { get; private set; }

        internal SourceRange(Interop.SourceRange native) {
            Native = native;
        }

        public SourceRange(SourceLocation start, SourceLocation end) {
            Native = Interop.clang_getRange(start.Native, end.Native);
        }

        public static SourceRange Null {
            get { return new SourceRange(Interop.clang_getNullRange()); }
        }

        public SourceLocation Start {
            get { return new SourceLocation(Interop.clang_getRangeStart(Native)); }
        }

        public SourceLocation End {
            get { return new SourceLocation(Interop.clang_getRangeEnd(Native)); }
        }
    }
}
