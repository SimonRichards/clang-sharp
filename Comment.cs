namespace ClangSharp {
    public class Comment {
        public SourceLocation Location { get; internal set; }
        public SourceRange Extent { get; internal set; }
        public string Spelling { get; internal set; }

        public bool IsSingleLine {
            get {
                return Spelling.StartsWith("//");
            }
        }

        public override string ToString() {
            return Spelling;
        }
    }
}
