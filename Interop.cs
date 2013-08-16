using System;
using System.Runtime.InteropServices;
using Index = System.IntPtr;
using File = System.IntPtr;
using TranslationUnit = System.IntPtr;
using ClientData = System.IntPtr;
using Diagnostic = System.IntPtr;
using CompletionString = System.IntPtr;

namespace ClangSharp {

    internal class Interop {

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal unsafe struct ClangString {
            readonly void* Spelling;
            readonly uint MustFreeString;

            public string ManagedString {
                get {
                    string res = Marshal.PtrToStringAnsi((IntPtr)Spelling);
                    Interop.clang_disposeString(this);
                    return res;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct UnsavedFile {
            internal string Filename;
            internal string Contents;
            internal ulong Length;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Type {
            internal ClangSharp.Type.Kind kind;
            readonly IntPtr data0, data1;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct SourceLocation {
            readonly IntPtr data0;
            readonly IntPtr data1;
            readonly uint data2;

            public bool Equals(SourceLocation other) {
                return data0 == other.data0 && data1 == other.data1 && data2 == other.data2;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SourceRange {
            readonly IntPtr data0;
            readonly IntPtr data1;
            readonly uint beginIntData;
            readonly uint endIntData;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Cursor {
            readonly CursorKind kind;
            readonly int xdata;
            readonly IntPtr data0, data1, data2;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct CompletionResult {
            internal CursorKind kind;
            internal CompletionString completionString;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct CodeCompleteResults {
            internal CompletionResult* results;
            internal uint NumResults;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Token {
            readonly uint data0, data1, data2, data3;
            readonly internal IntPtr ptr;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void InclusionVisitor(
            IntPtr includedFile,
            IntPtr inclusionStack,
            uint stackSize,
            ClientData clientData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate ClangSharp.Cursor.ChildVisitResult CursorVisitor(Cursor cursor, Cursor parent, ClientData data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ChildVistor(Cursor cursor, Cursor parent, ClientData clientData);

        const string nativeLib = "libclang";
        const CallingConvention convention = CallingConvention.Cdecl;
        const CharSet charSet = CharSet.Ansi;
        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern void clang_disposeString(ClangString str);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern IntPtr clang_createIndex(int excludeDeclarationsFromPch, int displayDiagnostics);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern void clang_disposeIndex(IntPtr index);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern void clang_setUseExternalASTGeneration(IntPtr index, int value);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getFileName(IntPtr file);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern long clang_getFileTime(IntPtr file);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern IntPtr clang_getFile(IntPtr tu, string filename);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern SourceLocation clang_getNullLocation();

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_equalLocations(SourceLocation loc1, SourceLocation loc2);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern SourceLocation clang_getLocation(IntPtr tu, IntPtr file, uint line, uint column);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern SourceRange clang_getNullRange();

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern SourceRange clang_getRange(SourceLocation begin, SourceLocation end);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public unsafe static extern void clang_getInstantiationLocation(
            SourceLocation location, IntPtr* file, out uint line, out uint column, out uint offset);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern SourceLocation clang_getRangeStart(SourceRange range);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern SourceLocation clang_getRangeEnd(SourceRange range);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_getNumDiagnostics(IntPtr tu);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern IntPtr clang_getDiagnostic(IntPtr tu, uint index);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern void clang_disposeDiagnostic(IntPtr diag);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_formatDiagnostic(
            IntPtr diag, ClangSharp.Diagnostic.DisplayOptions options);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_defaultDiagnosticDisplayOptions();

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangSharp.Diagnostic.Severity clang_getDiagnosticSeverity(IntPtr diag);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern SourceLocation clang_getDiagnosticLocation(IntPtr diag);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getDiagnosticSpelling(IntPtr diag);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_getDiagnosticNumRanges(IntPtr diag);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern SourceRange clang_getDiagnosticRange(IntPtr diag, uint range);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_getDiagnosticNumFixIts(IntPtr diag);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getDiagnosticFixIt(
            IntPtr diag, uint fixit, out SourceRange replacementRange);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getTranslationUnitSpelling(IntPtr tu);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern IntPtr clang_createTranslationUnitFromSourceFile(
            IntPtr index, string sourceFilename, int numClangCommandLineArgs,
            string[] clangCommandLineArgs, uint numUnsavedFiles, UnsavedFile[] unsavedFiles);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern IntPtr clang_createTranslationUnit(IntPtr index, string astFilename);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern IntPtr clang_parseTranslationUnit(
            IntPtr index, string filename, string[] args, int numArgs,
            IntPtr unsavedFiles, int numUnsavedFiles, uint options);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern void clang_disposeTranslationUnit(IntPtr tu);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_hashCursor(Cursor cursor);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern Cursor clang_getNullCursor();

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern Cursor clang_getTranslationUnitCursor(IntPtr tu);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_equalCursors(Cursor a, Cursor b);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern CursorKind clang_getCursorKind(Cursor c);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_isDeclaration(CursorKind ck);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_isReference(CursorKind ck);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_isExpression(CursorKind ck);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_isStatement(CursorKind ck);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_isAttribute(CursorKind ck);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_isInvalid(CursorKind ck);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_isTranslationUnit(CursorKind ck);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_isPreprocessing(CursorKind ck);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_isUnexposed(CursorKind ck);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern LinkageKind clang_getCursorLinkage(Cursor cursor);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern LanguageKind clang_getCursorLanguage(Cursor cursor);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern Cursor clang_getCursor(IntPtr tu, SourceLocation loc);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern SourceLocation clang_getCursorLocation(Cursor cursor);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern SourceRange clang_getCursorExtent(Cursor cursor);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern Type clang_getCursorType(Cursor c);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_equalTypes(Type a, Type b);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern Type clang_getCanonicalType(Type t);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern Type clang_getPointeeType(Type t);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern Cursor clang_getTypeDeclaration(Type t);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getTypeKindSpelling(ClangSharp.Type.Kind k);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern Type clang_getResultType(Type t);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern Type clang_getCursorResultType(Cursor c);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getCursorUSR(Cursor c);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_constructUSR_ObjCClass(string className);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_constructUSR_ObjCCategory(string className, string categoryName);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_constructUSR_ObjCProtocol(string protocolName);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_constructUSR_ObjCIvar(string name, ClangString classUSR);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_constructUSR_ObjCMethod(
            string name, uint isInstanceMethod, ClangString classUSR);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_constructUSR_ObjCProperty(string prop, ClangString classUSR);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getCursorSpelling(Cursor c);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern Cursor clang_getCursorReferenced(Cursor c);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern Cursor clang_getCursorDefinition(Cursor c);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_isCursorDefinition(Cursor c);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_CXXMethod_isStatic(Cursor c);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern TokenKind clang_getTokenKind(Token t);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getTokenSpelling(IntPtr tu, Token t);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern SourceLocation clang_getTokenLocation(IntPtr tu, Token t);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern SourceRange clang_getTokenExtent(IntPtr tu, Token t);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern void clang_tokenize(IntPtr tu, SourceRange range, out IntPtr tokens, out uint numTokens);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern void clang_annotateTokens(IntPtr tu, IntPtr tokens, uint numTokens, IntPtr cursors);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern void clang_disposeTokens(IntPtr tu, IntPtr tokens, uint numTokens);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getCursorKindSpelling(CursorKind ck);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern CodeCompletion.Chunk.ChunkKind clang_getCompletionChunkKind(CompletionString cs, uint chunk);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getCompletionChunkText(CompletionString cs, uint chunk);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern CompletionString clang_getCompletionChunkCompletionString(CompletionString cs, uint chunk);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_getNumCompletionChunks(CompletionString cs);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_getCompletionPriority(CompletionString cs);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern CodeCompletion.AvailabilityKind clang_getCompletionAvailability(CompletionString native);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_getCompletionNumAnnotations(CompletionString native);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getCompletionAnnotation(CompletionString native, uint index);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getCompletionAnnotation(CompletionString native);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public unsafe static extern CodeCompleteResults* clang_codeCompleteAt(
            IntPtr translationUnit, string filename, uint line, uint column,
            UnsavedFile[] unsavedFiles, uint numUnsavedFiles, uint options);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public unsafe static extern void clang_disposeCodeCompleteResults(CodeCompleteResults* results);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public unsafe static extern uint clang_codeCompleteGetNumDiagnostics(CodeCompleteResults* results);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public unsafe static extern IntPtr clang_codeCompleteGetDiagnostic(CodeCompleteResults* results, uint index);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_defaultCodeCompleteOptions();

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern IntPtr clang_getDiagnosticSetFromTU(IntPtr translationUnit);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_getNumDiagnosticsInSet(IntPtr diagnosticSet);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern IntPtr clang_getDiagnosticInSet(IntPtr diagnosticSet, uint index);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern ClangString clang_getClangVersion();

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern void clang_toggleCrashRecovery(uint isEnabled);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern void clang_getInclusions(IntPtr tu, InclusionVisitor visitor, ClientData data);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern uint clang_visitChildren(Cursor parent, CursorVisitor visitor, ClientData clientData);

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern IntPtr clang_Cursor_getTranslationUnit(Cursor cursor); // TODO Does this need disposing?

        [DllImport(nativeLib, CallingConvention = convention, CharSet = charSet)]
        public static extern int clang_Cursor_isNull(Cursor native);

    }
}