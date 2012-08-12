using System;
namespace ClangSharp {
    public class Cursor {
        internal Interop.Cursor Native { get; private set; }

        public CursorKind Kind { get; private set; }

        internal Cursor(Interop.Cursor native) {
            Native = native;
            Kind = Interop.clang_getCursorKind(Native);
        }

        public bool Equals(Cursor other) {
            return Interop.clang_equalCursors(Native, other.Native) == 0;
        }

        public override bool Equals(object obj) {
            return obj is Cursor && Equals((Cursor)obj);
        }

        public override int GetHashCode() {
            return (int)((long)Interop.clang_hashCursor(Native) - UInt32.MaxValue / 2);
        }

        public bool IsDeclaration {
            get { return Interop.clang_isDeclaration(Kind) != 0; }
        }

        public bool IsReference {
            get { return Interop.clang_isReference(Kind) != 0; }
        }

        public bool IsExpression {
            get { return Interop.clang_isExpression(Kind) != 0; }
        }

        public bool IsStatement {
            get { return Interop.clang_isStatement(Kind) != 0; }
        }

        public bool IsInvalid {
            get { return Interop.clang_isInvalid(Kind) != 0; }
        }

        public bool IsTranslationUnit {
            get { return Interop.clang_isTranslationUnit(Kind) != 0; }
        }

        public bool IsPreprocessing {
            get { return Interop.clang_isPreprocessing(Kind) != 0; }
        }

        public bool IsUnexposed {
            get { return Interop.clang_isUnexposed(Kind) != 0; }
        }

        public LinkageKind Linkage {
            get { return Interop.clang_getCursorLinkage(Native); }
        }

        public LanguageKind Language {
            get { return Interop.clang_getCursorLanguage(Native); }
        }

        public SourceLocation Location {
            get { return new SourceLocation(Interop.clang_getCursorLocation(Native)); }
        }

        public SourceRange Extent {
            get { return new SourceRange(Interop.clang_getCursorExtent(Native)); }
        }

        public Type Type {
            get { return new Type(Interop.clang_getCursorType(Native)); }
        }

        public Type ResultType {
            get { return new Type(Interop.clang_getCursorResultType(Native)); }
        }

        public string UnifiedSymbolResolution {
            get { return Interop.clang_getCursorUSR(Native).ManagedString; }
        }

        private string _spelling;
        public string Spelling {
            get { return _spelling ?? (_spelling = Interop.clang_getCursorSpelling(Native).ManagedString); }
        }
        public Cursor Referenced {
            get { return new Cursor(Interop.clang_getCursorReferenced(Native)); }
        }
        public Cursor Definition {
            get { return new Cursor(Interop.clang_getCursorDefinition(Native)); }
        }
        public bool IsDefinition {
            get { return Interop.clang_isCursorDefinition(Native) != 0; }
        }
        public bool IsStaticCxxMethod {
            get { return Interop.clang_CXXMethod_isStatic(Native) != 0; }
        }

        public enum ChildVisitResult {
            Break,
            Continue,
            Recurse,
        };

        public delegate ChildVisitResult CursorVisitor(Cursor cursor, Cursor parent);

        public void VisitChildren(CursorVisitor visitor) {
            Interop.clang_visitChildren(
                Native,
                (cursor, parent, data) => visitor(new Cursor(cursor), new Cursor(parent)),
                IntPtr.Zero);
        }

        public override string ToString() {
            return Spelling + " is " + Kind + " in " + Location;
        }
    }
}
