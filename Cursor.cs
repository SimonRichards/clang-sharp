using System;
using System.Text;

namespace ClangSharp {
    public class Cursor {
        internal Interop.Cursor Native { get; private set; }

        /// <summary>
        /// Retrieve the kind of the given cursor. 
        /// </summary>
        public CursorKind Kind { get; private set; }

        internal Cursor(Interop.Cursor native) {
            Native = native;
            Kind = Interop.clang_getCursorKind(Native);
        }

        /// <summary>
        /// Determine whether two cursors are equivalent. 
        /// </summary>
        /// <param name="other">The cursor to compare with.</param>
        /// <returns></returns>
        public bool Equals(Cursor other) {
            return Interop.clang_equalCursors(Native, other.Native) != 0;
        }

        public override bool Equals(object obj) {
            return obj is Cursor && Equals((Cursor)obj);
        }

        public override int GetHashCode() {
            return (int)((long)Interop.clang_hashCursor(Native) - UInt32.MaxValue / 2);
        }

        public Cursor GetArgument(uint i)
        {
            return new Cursor(Interop.clang_Cursor_getArgument(Native, i));
        }

        public Type GetArgumentType(uint i)
        {
            return new Type(Interop.clang_getArgType(Native, i));
        }

        public Cursor GetOverloadedDecl(uint i)
        {
            return new Cursor(Interop.clang_getOverloadedDecl(Native, i));
        }

        /// <summary>
        /// Determine whether the cursor represents a declaration. 
        /// </summary>
        public bool IsDeclaration {
            get { return Interop.clang_isDeclaration(Kind) != 0; }
        }

        /// <summary>
        /// Determine whether the cursor represents a simple reference. 
        /// </summary>
        public bool IsReference {
            get { return Interop.clang_isReference(Kind) != 0; }
        }

        /// <summary>
        /// Determine whether the cursor represents an expression. 
        /// </summary>
        public bool IsExpression {
            get { return Interop.clang_isExpression(Kind) != 0; }
        }

        /// <summary>
        /// Determine whether the cursor represents a statement. 
        /// </summary>
        public bool IsStatement {
            get { return Interop.clang_isStatement(Kind) != 0; }
        }

        /// <summary>
        /// Determine whether the cursor represents an attribute. 
        /// </summary>
        public bool IsAttribute {
            get { return Interop.clang_isAttribute(Kind) != 0; }
        }

        /// <summary>
        /// Determine whether the cursor represents an invalid cursor. 
        /// </summary>
        public bool IsInvalid {
            get { return Interop.clang_isInvalid(Kind) != 0; }
        }

        /// <summary>
        /// Determine whether the cursor represents a translation unit. 
        /// </summary>
        public bool IsTranslationUnit {
            get { return Interop.clang_isTranslationUnit(Kind) != 0; }
        }

        public bool IsPreprocessing {
            get { return Interop.clang_isPreprocessing(Kind) != 0; }
        }

        public bool IsUnexposed {
            get { return Interop.clang_isUnexposed(Kind) != 0; }
        }

        public int NumArguments
        {
            get { return Interop.clang_Cursor_getNumArguments(Native); }
        }

        public uint NumOverloadedDecls
        {
            get { return Interop.clang_getNumOverloadedDecls(Native); }
        }

        /// <summary>
        /// Determine the linkage of the entity referred to by a given cursor. 
        /// </summary>
        public LinkageKind Linkage {
            get { return Interop.clang_getCursorLinkage(Native); }
        }

        /// <summary>
        /// Determine the "language" of the entity referred to by a given cursor. 
        /// </summary>
        public LanguageKind Language {
            get { return Interop.clang_getCursorLanguage(Native); }
        }

        /// <summary>
        /// Retrieve the physical location of the source constructor referenced by the given cursor.
        /// The location of a declaration is typically the location of the name of that declaration,
        /// where the name of that declaration would occur if it is unnamed, or some keyword that
        /// introduces that particular declaration. The location of a reference is where that reference
        /// occurs within the source code.
        /// </summary>
        public SourceLocation Location {
            get { return new SourceLocation(Interop.clang_getCursorLocation(Native)); }
        }

        /// <summary>
        /// Retrieve the physical extent of the source construct referenced by the given cursor.
        /// The extent of a cursor starts with the file/line/column pointing at the first character
        /// within the source construct that the cursor refers to and ends with the last character
        /// withinin that source construct. For a declaration, the extent covers the declaration itself.
        /// For a reference, the extent covers the location of the reference (e.g., where the
        /// referenced entity was actually used).
        /// </summary>
        public SourceRange Extent {
            get { return new SourceRange(Interop.clang_getCursorExtent(Native)); }
        }

        public Type Type {
            get { return new Type(Interop.clang_getCursorType(Native)); }
        }

        public Type TypedefDeclUnderlyingType
        {
            get { return new Type(Interop.clang_getTypedefDeclUnderlyingType(Native)); }
        }

        public Type EnumDeclIntegerType
        {
            get { return new Type(Interop.clang_getEnumDeclIntegerType(Native)); }
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

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName ?? (_displayName = Interop.clang_getCursorDisplayName(Native).ManagedString); }
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

        public bool IsPureVirtualCxxMethod
        {
            get { return Interop.clang_CXXMethod_isPureVirtual(Native) != 0; }
        }

        public bool IsVirtualCxxMethod
        {
            get { return Interop.clang_CXXMethod_isVirtual(Native) != 0; }
        }
        public CursorKind TemplateCursorKind
        {
            get { return Interop.clang_getTemplateCursorKind(Native); }
        }
        public Cursor SpecializedCursorTemplate
        {
            get { return new Cursor(Interop.clang_getSpecializedCursorTemplate(Native)); }
        }
        public SourceRange CursorReferenceNameRange
        {
            get { return new SourceRange(Interop.clang_getCursorReferenceNameRange(Native)); }
        }

        public AccessSpecifier AccessSpecifier
        {
            get { return Interop.clang_getCXXAccessSpecifier(Native); }
        }

        public bool IsBitField
        {
            get { return Interop.clang_Cursor_isBitField(Native) != 0; }
        }

        public bool IsDynamicCall
        {
            get { return Interop.clang_Cursor_isDynamicCall(Native) != 0; }
        }

        public bool IsNull {
            get { return Interop.clang_Cursor_isNull(Native) != 0; }
        }

        public bool IsObjCOptional
        {
            get { return Interop.clang_Cursor_isObjCOptional(Native) != 0; }
        }

        public bool IsVariadic
        {
            get { return Interop.clang_Cursor_isVariadic(Native) != 0; }
        }

        public bool IsVirtualBase
        {
            get { return Interop.clang_isVirtualBase(Native) != 0; }
        }

        public Cursor LexicalParent
        {
            get { return new Cursor(Interop.clang_getCursorLexicalParent(Native)); }
        }

        public Cursor SemanticParent
        {
            get { return new Cursor(Interop.clang_getCursorSemanticParent(Native)); }
        }

        /// <summary>
        /// Retrieve the NULL cursor, which represents no entity. 
        /// </summary>
        public static Cursor Null {
            get { return new Cursor(Interop.clang_getNullCursor()); }
        }

        /// <summary>
        /// Attempts to retrieve the source code this cursor points to.
        /// </summary>
        /// <remarks>
        /// Method is not necessarily performant, for debugging only.
        /// </remarks>
        public string Source {
            get {
                var tu = new TranslationUnit(Interop.clang_Cursor_getTranslationUnit(Native));
                var path = tu.Spelling;
                if (System.IO.File.Exists(path)) {
                    return System.IO.File.ReadAllText(path, Encoding.ASCII).Substring(Extent);
                } else {
                    return string.Empty;
                }
            }
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