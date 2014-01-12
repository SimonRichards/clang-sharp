namespace ClangSharp {
    public class Token {

        public TokenKind Kind { get; private set; }
        public string Spelling { get; private set; }
        public SourceLocation Location { get; private set; }
        public SourceRange Extent { get; private set; }

        internal Token(Interop.Token native, TranslationUnit parent) {
            Kind = Interop.clang_getTokenKind(native);
            Location = new SourceLocation(Interop.clang_getTokenLocation(parent.Native, native));
            Extent = new SourceRange(Interop.clang_getTokenExtent(parent.Native, native));
            Spelling = Interop.clang_getTokenSpelling(parent.Native, native).ManagedString; //parent.GetText(Extent);
        }
    }
}