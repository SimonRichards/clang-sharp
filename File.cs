using System;

namespace ClangSharp {
    public class File {
        internal IntPtr Native { get; private set; }

        internal File(IntPtr native) {
            Native = native;
        }

        public string Name {
            get { return Interop.clang_getFileName(Native).ManagedString ?? string.Empty; }
        }

        public DateTime Time {
            get {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddSeconds(Interop.clang_getFileTime(Native));
            }
        }

        public override string ToString() {
            return Name;
        }

        public static bool operator ==(File first, File second) {
            return first.Native == second.Native;
        }

        public static bool operator !=(File first, File second) {
            return first.Native != second.Native;
        }

        public override int GetHashCode() {
            return Native.GetHashCode();
        }

        public override bool Equals(object obj) {
            return Native.Equals(((File)obj).Native);
        }
    }
}