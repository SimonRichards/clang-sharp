using System;
using System.Collections.Generic;

namespace ClangSharp {
    public class Diagnostic : IDisposable {

        [Flags]
        public enum DisplayOptions {
            DisplaySourceLocation = 1,
            DisplayColumn = 2,
            DisplaySourceRanges = 4
        };

        public static DisplayOptions DefaultDisplayOptions =
            (DisplayOptions)Interop.clang_defaultDiagnosticDisplayOptions();

        public enum Severity { Ignored, Note, Warning, Error, Fatal }

        internal IntPtr Native { get; private set; }

        public Severity Level {
            get { return Interop.clang_getDiagnosticSeverity(Native); }
        }

        public SourceLocation Location {
            get { return new SourceLocation(Interop.clang_getDiagnosticLocation(Native)); }
        }

        public string Spelling {
            get { return Interop.clang_getDiagnosticSpelling(Native).ManagedString; }
        }

        public uint NumRanges {
            get { return Interop.clang_getDiagnosticNumRanges(Native); }
        }

        public IEnumerable<SourceRange> GetRanges() {
            uint count = NumRanges;
            for (uint i = 0; i < count; ++i) {
                yield return new SourceRange(Interop.clang_getDiagnosticRange(Native, i));
            }
        }

        public uint NumFixits {
            get { return Interop.clang_getDiagnosticNumFixIts(Native); }
        }

        public IEnumerable<FixIt> Fixits {
            get {
                uint count = NumFixits;
                for (uint i = 0; i < count; ++i) {
                    Interop.SourceRange range;
                    string spelling = Interop.clang_getDiagnosticFixIt(Native, i, out range).ManagedString;
                    yield return new FixIt {Fix = spelling, Range = new SourceRange(range)};
                }
            }
        }

        internal Diagnostic(IntPtr native) {
            Native = native;
        }

        public string Format() {
            return Format(DefaultDisplayOptions);
        }

        public string Format(DisplayOptions options) {
            return Interop.clang_formatDiagnostic(Native, options).ManagedString;
        }

        public void Dispose() {
            Interop.clang_disposeDiagnostic(Native);
        }
    }
}
