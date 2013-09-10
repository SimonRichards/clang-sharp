using System;
using System.Text;

namespace ClangSharp {
    public class SourceLocation : IComparable {
        internal Interop.SourceLocation Native { get; private set; }

        public File File { get; private set; }
        public readonly int Line;
        public readonly int Column;
        public readonly int Offset;

        internal unsafe SourceLocation(Interop.SourceLocation native) {
            Native = native;
            IntPtr file = IntPtr.Zero;
            uint line, column, offset;
            Interop.clang_getInstantiationLocation(Native, &file, out line, out column, out offset);
            Line = (int)line;
            Column = (int)column;
            Offset = (int)offset;
            File = new File(file);
        }

        public int OffsetAtStartOfLine {
            get {
                return Offset - Column + 1;
            }
        }

        public static SourceLocation Null {
            get { return new SourceLocation(Interop.clang_getNullLocation()); }
        }

        public bool Equals(SourceLocation other) {
            return this == other;
        }

        public bool IsValid {
            get {
                return this != Null && !string.IsNullOrEmpty(File.Name);
            }
        }

        public static bool operator ==(SourceLocation left, SourceLocation right) {
            return left.File == right.File && left.Offset == right.Offset;
        }

        public static bool operator !=(SourceLocation left, SourceLocation right) {
            return left.File != right.File || left.Offset != right.Offset;
        }

        public override bool Equals(object obj) {
            return obj is SourceLocation && Equals((SourceLocation)obj);
        }

        public override int GetHashCode() {
            return (File.ToString() + Offset).GetHashCode();
        }

        public override string ToString() {
            return string.Format("{0}({1},{2})", File, Line, Column);
        }

        public int CompareTo(object obj) {
            SourceLocation other = obj as SourceLocation;
            if (other == null) {
                throw new ArgumentException("Other object in comparison was of wrong type", "obj");
            }
            return Offset.CompareTo(other.Offset);
        }

        public static bool operator <(SourceLocation first, SourceLocation second) {
            return first.Offset < second.Offset;
        }

        public static bool operator >(SourceLocation first, SourceLocation second) {
            return first.Offset > second.Offset;
        }
    }
}