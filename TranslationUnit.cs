using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ClangSharp {
    public class TranslationUnit : IDisposable {

        internal IntPtr Native { get; private set; }
        private readonly string _filename;
        private byte[] _bytes;
        private readonly int _bomLength;
        private readonly List<int> _lineStarts = new List<int>();
        private readonly List<int> _multiByteChars = new List<int>();

        private static readonly Regex _startsWithWhitespace = new Regex(@"^\s*(\/\/|\/\*)", RegexOptions.Compiled);

        internal TranslationUnit(string filename, IntPtr native)
            : this(native, filename) {
        }

        internal TranslationUnit(IntPtr native)
            : this(native, "") {
            _filename = Spelling;
        }

        private TranslationUnit(IntPtr native, string name) {
            _filename = name;
            Native = native;
            if (native == IntPtr.Zero) {
                throw new ClangException("Failed to parse: {0}", name);
            }
            _bytes = System.IO.File.ReadAllBytes(name);
            _bomLength = _bytes.GetBomLength();
            using (var reader = new StreamReader(new MemoryStream(_bytes))) {
                Source = reader.ReadToEnd();
                Encoding = reader.CurrentEncoding;
            }
            for (int i = _bomLength; i < _bytes.Length; ++i) {
                byte b = _bytes[i];
                if ((b == '\n' || b == '\r') && !(b == '\r' && i + 1 < _bytes.Length && _bytes[i + 1] == '\n')) {
                    _lineStarts.Add(i + 1 - _bomLength);
                }
                if ((b & 0xC0) == 0x80) {
                    _multiByteChars.Add(i - _bomLength);
                }
            }
            AsciiSource = Encoding.ASCII.GetString(_bytes);
        }

        private static readonly Regex _lineSplit = new Regex("\r\n|\r|\n", RegexOptions.Compiled);
        private string[] _lines;
        public string GetLine(int line) {
            _lines = _lines ?? _lineSplit.Split(AsciiSource);
            return _lines[line - 1];
        }

        public string AsciiSource { get; private set; }

        public string Source { get; private set; }

        public Encoding Encoding { get; private set; }

        public string Spelling {
            get { return Interop.clang_getTranslationUnitSpelling(Native).ManagedString; }
        }

        public Cursor Cursor {
            get { return new Cursor(Interop.clang_getTranslationUnitCursor(Native)); }
        }

        public bool IsFileMultipleIncludeGuarded(File file)
        {
            return Interop.clang_isFileMultipleIncludeGuarded(Native, file.Native) != 0;
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
            GC.SuppressFinalize(this);
        }

        ~TranslationUnit() {
            //Interop.clang_disposeIndex(Native);
        }

        /// <summary>
        /// Map a source location to the cursor that describes the entity at that location in the source code.
        /// GetCursor() maps an arbitrary source location within a translation unit down to the most specific
        /// cursor that describes the entity at that location. For example, given an expression x + y, invoking
        /// GetCursor() with a source location pointing to "x" will return the cursor for "x"; similarly for "y".
        /// If the cursor points anywhere between "x" or "y" (e.g., on the + or the whitespace around it),
        /// GetCursor() will return a cursor referring to the "+" expression.
        /// </summary>
        /// <param name="location">The location to find a cursor at.</param>
        /// <returns></returns>
        public Cursor GetCursor(SourceLocation location) {
            return new Cursor(Interop.clang_getCursor(Native, location.Native));
        }

        public IEnumerable<Token> Tokenize() {
            return Tokenize(FullRange);
        }

        /// <summary>
        /// Tokenize the source code described by the given range into raw lexical tokens. 
        /// </summary>
        /// <param name="range">The range to tokenize.</param>
        /// <returns>A list of raw lexical tokens.</returns>
        public IEnumerable<Token> Tokenize(SourceRange range, TokenKind filter = TokenKind.All) {
            IntPtr tokenPtr;
            uint numTokens;
            Interop.clang_tokenize(Native, range.Native, out tokenPtr, out numTokens);
            var tokens = new Interop.Token[numTokens];
            try {
                unsafe {
                    for (uint i = 0; i < numTokens; ++i) {
                        tokens[i] = *(Interop.Token*)(tokenPtr + (int)(i * sizeof(Interop.Token)));
                    }
                    return tokens
                        .Where(t => filter == TokenKind.All || Interop.clang_getTokenKind(t) == filter)
                        .Select(token => new Token(token, this));
                }
            } finally {
                Interop.clang_disposeTokens(Native, tokenPtr, numTokens);
            }
        }

        public delegate void InclusionVisitor(File included, SourceLocation[] inclusionStack);

        /// <summary>
        /// Visit the set of preprocessor inclusions in a translation unit. The visitor
        /// function is called with the provided data for every included file. This does
        /// not include headers included by the PCH file (unless one is inspecting the
        /// inclusions in the PCH file itself).
        /// </summary>
        /// <param name="visitor">The visiting delegate.</param>
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

        public SourceLocation Start {
            get {
                if (string.IsNullOrEmpty(_filename)) {
                    throw new InvalidOperationException("Can't get a location without the filename");
                }
                return GetLocation(GetFile(_filename), 1, 1);
            }
        }

        public SourceLocation End {
            get {
                if (string.IsNullOrEmpty(_filename)) {
                    throw new InvalidOperationException("Can't get a location without the filename");
                }
                try {
                    string[] source = System.IO.File.ReadAllLines(_filename);
                    return GetLocation(GetFile(_filename), (uint)source.Length + 1, (uint)source.Last().Length + 1);
                } catch {
                    return GetLocation(GetFile(_filename), 1, 1);
                }

            }
        }

        public SourceRange FullRange {
            get {
                return new SourceRange(Start, End);
            }
        }

        public string GetText(SourceRange extent) {
            return Encoding.GetString(_bytes.Slice(extent));
        }

        public string GetText(int start, int length) {
            int trim = Math.Max(0, _bomLength - start);
            start += trim;
            length -= trim;
            return Encoding.GetString(_bytes.Slice(start, length));
        }

        /// <summary>
        /// Convert a byte offset into a (utf16) char offset, line and column numbers.
        /// </summary>
        /// <param name="byteOffset">The byte offset (as given by libclang).</param>
        /// <param name="line">The zero-based line number.</param>
        /// <param name="column">The zero-based column number.</param>
        /// <param name="offset">The zero-based offset.</param>
        public void GetCharLocations(int byteOffset, out int line, out int column, out int offset) {
            int lineStart, last = 0;
            line = 0;
            column = offset = byteOffset;
            foreach (int start in _lineStarts) {
                if (start > byteOffset) {
                    break;
                } else {
                    ++line;
                }
                last = lineStart = start;
            }
            column = byteOffset - last;
            foreach (int multiByteChar in _multiByteChars) {
                if (multiByteChar > last) {
                    --column;
                }
                if (multiByteChar > byteOffset) {
                    break;
                }
                last = multiByteChar;
            }
        }

        public int GetLineNumber(int offset) {
            int line = 0;
            foreach (int start in _lineStarts) {
                if (start > offset) {
                    return line;
                }
                ++line;
            }
            return line;
        }

        /// <summary>
        /// Retrieves a list of comments.
        /// </summary>
        /// <remarks>
        /// Single line comments which start with nothing or whitespace are merged.
        /// </remarks>
        public IList<Comment> Comments {
            get {
                var tokens = Tokenize(FullRange, TokenKind.Comment);
                var comments = (
                    from token in tokens
                    where token.Kind == TokenKind.Comment
                    select new Comment {
                        Extent = token.Extent,
                        Location = token.Location,
                        Spelling = token.Spelling
                    }).ToList();

                if (string.IsNullOrEmpty(AsciiSource) || comments.Count <= 1) {
                    return comments;
                }

                IList<Comment> mergedComments = new List<Comment>();
                Func<Comment, bool> startsWithWhitespace = comment => _startsWithWhitespace.IsMatch(GetLine(comment.Location.Line));

                for (int i = 0; i < comments.Count - 1; ++i) {
                    var first = comments[i];
                    var second = comments[i + 1];
                    if (first.Location.Line == second.Location.Line - 1 &&
                        startsWithWhitespace(first) &&
                        startsWithWhitespace(second)) {
                        var extent = new SourceRange(first.Extent.Start, second.Extent.End);
                        mergedComments.Add(new Comment {
                            Extent = extent,
                            Location = first.Location,
                            Spelling = GetText(extent)
                        });
                        ++i;
                    } else {
                        mergedComments.Add(first);
                        if (i == comments.Count - 2) {
                            mergedComments.Add(second);
                        }
                    }
                }
                return mergedComments;
            }
        }
    }
}