using System;
using System.Collections.Generic;
using System.Linq;

namespace ClangSharp {
    public class TranslationUnit : IDisposable {
        internal IntPtr Native { get; private set; }

        internal TranslationUnit(IntPtr native) {
            if (native == IntPtr.Zero) {
                throw new ArgumentException("pointer was null");
            }
            Native = native;
        }

        public string Spelling {
            get { return Interop.clang_getTranslationUnitSpelling(Native).ManagedString; }
        }

        public Cursor Cursor {
            get { return new Cursor(Interop.clang_getTranslationUnitCursor(Native)); }
        }

        public File GetFile(string filename) {
            return new File(Interop.clang_getFile(Native, filename));
        }

        public SourceLocation GetLocation(File file, uint line, uint column) {
            return new SourceLocation(Interop.clang_getLocation(Native, file.Native, line, column));
        }

        public uint NumDiagnostics {
            get { return Interop.clang_getNumDiagnostics(Native); }
        }

        public IEnumerable<Diagnostic> Diagnostics {
            get {
                IntPtr diagnosticSet = Interop.clang_getDiagnosticSetFromTU(Native);
                uint count = Interop.clang_getNumDiagnosticsInSet(diagnosticSet);
                for (uint i = 0; i < count; ++i) {
                    yield return new Diagnostic(Interop.clang_getDiagnosticInSet(diagnosticSet, i));
                }
            }
        }

        public void Dispose() {
            Interop.clang_disposeTranslationUnit(Native);
        }

        public Cursor GetCursor(SourceLocation location) {
            return new Cursor(Interop.clang_getCursor(Native, location.Native));
        }

        public IList<Token> Tokenize(SourceRange range) {
            IntPtr tokenPtr;
            uint numTokens;
            Interop.clang_tokenize(Native, range.Native, out tokenPtr, out numTokens);
            var tokens = new Interop.Token[numTokens];
            unsafe {
                for (uint i = 0; i < numTokens; ++i) {
                    tokens[i] = *(Interop.Token*)(tokenPtr + (int)(i*sizeof (Interop.Token)));
                }
            }
            return tokens.Select(token => new Token(token, this)).ToList();
        }

        public delegate void InclusionVisitor(File included, SourceLocation[] inclusionStack);

        public void VisitInclusions(InclusionVisitor visitor) {
            Interop.InclusionVisitor nativeVisitor = (file, stack, size, data) => {
                var inclusionStack = new SourceLocation[(int)size];
                unsafe {
                    var srcPtr = (Interop.SourceLocation*)stack;
                    for (uint i = 0; i < size; ++i) {
                        inclusionStack[i] = (new SourceLocation(*(srcPtr + i)));
                    }
                }
                visitor(new File(file), inclusionStack);
            };
            Interop.clang_getInclusions(Native, nativeVisitor, IntPtr.Zero);
        }

        public IList<CodeCompletion> CodeCompleteAt(uint line, uint column, ICollection<UnsavedFile> unsavedFiles) {
            return CodeCompleteAt(line, column, unsavedFiles, CodeCompletion.DefaultOptions());
        }

        public IList<CodeCompletion> CodeCompleteAt(uint line, uint column, ICollection<UnsavedFile> unsavedFiles, CodeCompletion.Options options) {
            ICollection<Diagnostic> diagnostics;
            return CodeCompleteAt(line, column, unsavedFiles, (uint)options, out diagnostics);
        }

        public IList<CodeCompletion> CodeCompleteAt(uint line, uint column, ICollection<UnsavedFile> unsavedFiles, out ICollection<Diagnostic> diagnostics) {
            return CodeCompleteAt(line, column, unsavedFiles, CodeCompletion.DefaultOptions(), out diagnostics);
        }

        public IList<CodeCompletion> CodeCompleteAt(uint line, uint column, ICollection<UnsavedFile> unsavedFiles, CodeCompletion.Options options, out ICollection<Diagnostic> diagnostics) {
            return CodeCompleteAt(line, column, unsavedFiles, (uint)options, out diagnostics);
        }

        private unsafe IList<CodeCompletion> CodeCompleteAt(uint line, uint column, ICollection<UnsavedFile> unsavedFiles, uint options, out ICollection<Diagnostic> diagnostics) {
            var nativeCompletions = Interop.clang_codeCompleteAt(
                Native,
                Spelling,
                line,
                column,
                unsavedFiles.Select(file => file.Native).ToArray(),
                (uint)unsavedFiles.Count,
                options);

            uint numDiagnostics = Interop.clang_codeCompleteGetNumDiagnostics(nativeCompletions);
            diagnostics = new List<Diagnostic>((int)numDiagnostics);
            for (uint i = 0; i < numDiagnostics; ++i) {
                diagnostics.Add(new Diagnostic(Interop.clang_codeCompleteGetDiagnostic(nativeCompletions, i)));
            }

            var completions = new List<CodeCompletion>((int)nativeCompletions->NumResults);
            for (uint i = 0; i < nativeCompletions->NumResults; ++i) {
                completions.Add(new CodeCompletion(nativeCompletions->results + i));
            }
            Interop.clang_disposeCodeCompleteResults(nativeCompletions);
            return completions;
        }
    }
}
