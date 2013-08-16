using System;
using System.Collections.Generic;
using System.Linq;

namespace ClangSharp {
    public class Diagnostic {

        [Flags]
        public enum DisplayOptions {
            DisplaySourceLocation = 1,
            DisplayColumn = 2,
            DisplaySourceRanges = 4
        };

        public static DisplayOptions DefaultDisplayOptions =
            (DisplayOptions)Interop.clang_defaultDiagnosticDisplayOptions();

        public enum Severity { Ignored, Note, Warning, Error, Fatal }

        public readonly Severity Level;
        public readonly SourceLocation Location;
        public readonly string Spelling;
        public readonly IList<SourceRange> Ranges;
        public readonly IList<FixIt> Fixits;
        public readonly string Description;

        internal Diagnostic(IntPtr native) {
            Level = Interop.clang_getDiagnosticSeverity(native);
            Location = new SourceLocation(Interop.clang_getDiagnosticLocation(native));
            Spelling = Interop.clang_getDiagnosticSpelling(native).ManagedString;
            Ranges = Util.Range(0u, Interop.clang_getDiagnosticNumRanges(native)).Select(i =>
                new SourceRange(Interop.clang_getDiagnosticRange(native, i))).ToArray();
            uint numFixits = Interop.clang_getDiagnosticNumFixIts(native);
            Fixits = Util.Range(0u, Interop.clang_getDiagnosticNumFixIts(native)).Select(i => {
                Interop.SourceRange range;
                string spelling = Interop.clang_getDiagnosticFixIt(native, i, out range).ManagedString;
                return new FixIt { Fix = spelling, Range = new SourceRange(range) };
            }).ToArray();
            Description = Interop.clang_formatDiagnostic(native, DefaultDisplayOptions).ManagedString;
            Interop.clang_disposeDiagnostic(native);
        }
    }
}
