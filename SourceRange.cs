namespace ClangSharp {
    public class SourceRange {

        internal Interop.SourceRange Native { get; private set; }

        internal SourceRange(Interop.SourceRange native) {
            Native = native;
            var start = new SourceLocation(Interop.clang_getRangeStart(Native));
            var end = new SourceLocation(Interop.clang_getRangeEnd(Native));
            if (end < start) {
                Start = end;
                End = start;
            } else {
                Start = start;
                End = end;
            }
        }

        public SourceRange(SourceLocation start, SourceLocation end) :
            this(Interop.clang_getRange(start.Native, end.Native)) { }

        public static SourceRange Null {
            get { return new SourceRange(Interop.clang_getNullRange()); }
        }

        public SourceLocation Start { get; private set; }

        public SourceLocation End { get; private set; }

        public bool Equals(SourceRange other) {
            return Start == other.Start && End == other.End;
        }

        public override bool Equals(object obj) {
            return obj is SourceRange && Equals((SourceRange)obj);
        }

        public override int GetHashCode() {
            return Start.GetHashCode() + End.GetHashCode();
        }
    }
}