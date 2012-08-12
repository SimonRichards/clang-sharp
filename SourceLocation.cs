using System;

namespace ClangSharp {
    public class SourceLocation {
        internal Interop.SourceLocation Native { get; private set; }

        public File File { get; private set; }
        public readonly uint Line;
        public readonly uint Column;
        public readonly uint Offset;

        internal unsafe SourceLocation(Interop.SourceLocation native) {
            Native = native;
            IntPtr file = IntPtr.Zero;
            Interop.clang_getInstantiationLocation(Native, &file, out Line, out Column, out Offset);
            File = new File(file);
        }

        public static SourceLocation Null {
            get { return new SourceLocation(Interop.clang_getNullLocation()); }
        }

        public bool Equals(SourceLocation other) {
            return Native.Equals(other.Native);
        }

        public override bool Equals(object obj) {
            return obj is SourceLocation && Equals((SourceLocation)obj);
        }

        public override int GetHashCode() {
            return Native.GetHashCode();
        }

        public override string ToString() {
            return File.Name == "" ? "unknown file" : File + "+" + Line + ":" + Column;
        }
    }
}
