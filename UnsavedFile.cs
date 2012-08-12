namespace ClangSharp {
    public class UnsavedFile {
        internal Interop.UnsavedFile Native { get; private set; }

        public string Contents { get { return Native.Contents; } }
        public string Filename { get { return Native.Filename; } }
        public int Length { get { return (int)Native.Length; } }

        public UnsavedFile(string contents, string filename, int length) {
            Native = new Interop.UnsavedFile {
                Contents = contents,
                Filename = filename,
                Length = (uint)length
            };
        }
    }
}
