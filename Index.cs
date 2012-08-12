using System;
using System.Linq;

namespace ClangSharp {
    public class Index : IDisposable {
        internal readonly IntPtr Native;

        public Index() :this(true, true) {
        }

        public Index(bool excludeDeclarationsFromPch, bool displayDiagnostics) {
            Native = Interop.clang_createIndex(excludeDeclarationsFromPch ? 1 : 0, displayDiagnostics ? 1 : 0);
        }

        public void Dispose() {
            Interop.clang_disposeIndex(Native);
        }

        public bool ExternalAstGeneration {
            set {
                Interop.clang_setUseExternalASTGeneration(Native, value ? 1 : 0);
            }
        }

        public TranslationUnit CreateTranslationUnit(string astFilename) {
            return new TranslationUnit(Interop.clang_createTranslationUnit(Native, astFilename));
        }

        public TranslationUnit CreateTranslationUnit(string filename, Options[] clangArgs, UnsavedFile[] unsavedFiles) {
            var args = clangArgs.Select(arg => "-" + arg.ToString().Replace("_", "-")).ToArray();
            return CreateTranslationUnit(
                filename,
                args,
                unsavedFiles);
        }

        public TranslationUnit CreateTranslationUnit(string filename, string []clangArgs, UnsavedFile[] unsavedFiles) {
            return new TranslationUnit(
                Interop.clang_createTranslationUnitFromSourceFile(
                    Native,
                    filename,
                    clangArgs.Length,
                    clangArgs,
                    (uint)unsavedFiles.Length,
                    unsavedFiles.Select(f => f.Native).ToArray()));
        }
    }
}
