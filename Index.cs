using System;
using System.Linq;

namespace ClangSharp {
    public class Index : IDisposable {
        internal readonly IntPtr Native;

        public Index()
            : this(true, false) {
        }

        public Index(bool excludeDeclarationsFromPch, bool displayDiagnostics) {
            Native = Interop.clang_createIndex(excludeDeclarationsFromPch ? 1 : 0, displayDiagnostics ? 1 : 0);
        }

        public void Dispose() {
            Interop.clang_disposeIndex(Native);
            GC.SuppressFinalize(this);
        }

        ~Index() {
            Interop.clang_disposeIndex(Native);
        }

        public bool ExternalAstGeneration {
            set {
                Interop.clang_setUseExternalASTGeneration(Native, value ? 1 : 0);
            }
        }

        public TranslationUnit CreateTranslationUnitFromAst(string astFilename) {
            return new TranslationUnit(astFilename, Interop.clang_createTranslationUnit(Native, astFilename));
        }

        public TranslationUnit CreateTranslationUnit(string filename, Options[] clangArgs, UnsavedFile[] unsavedFiles = null, TranslationUnitFlags options = TranslationUnitFlags.None) {
            var args = clangArgs.Select(arg => "-" + arg.ToString().Replace("_", "-")).ToArray();
            return CreateTranslationUnit(
                filename,
                args,
                unsavedFiles,
                options);
        }

        public TranslationUnit CreateTranslationUnit(string filename, string[] clangArgs = null, UnsavedFile[] unsavedFiles = null, TranslationUnitFlags options = TranslationUnitFlags.None) {
            if (!System.IO.File.Exists(filename)) {
                throw new System.IO.FileNotFoundException("Couldn't find input file.", filename);
            }
            clangArgs = clangArgs ?? new string[0];
            unsavedFiles = unsavedFiles ?? new UnsavedFile[0];
            return new TranslationUnit(
                filename,
                Interop.clang_parseTranslationUnit(
                    Native,
                    filename,
                    clangArgs,
                    clangArgs.Length,
                    unsavedFiles.Select(f => f.Native).ToArray(),
                    (uint)unsavedFiles.Length,
                    options));
        }
    }

    [Flags]
    public enum TranslationUnitFlags
    {
        None = 0x00,
        DetailedPreprocessingRecord = 0x01,
        Incomplete = 0x02,
        PrecompiledPreamble = 0x04,
        CacheCompletionResults = 0x08,
        ForSerialization = 0x10,
        CXXChainedPCH = 0x20,
        SkipFunctionBodies = 0x40,
        IncludeBriefCommentsInCodeCompletion = 0x80
    }
}