using System;
using System.Collections.Generic;
using System.Linq;

namespace ClangSharp {
    public class CodeCompletion {
        public class Chunk {
            public enum ChunkKind {
                Optional,
                TypedText,
                Text,
                Placeholder,
                Informative,
                CurrentParameter,
                LeftParen,
                RightParen,
                LeftBracket,
                RightBracket,
                LeftBrace,
                RightBrace,
                LeftAngle,
                RightAngle,
                Comma,
                ResultType,
                Colon,
                Semicolon,
                Equal,
                HorizontalSpace,
                VerticalSpace,
            }

            public readonly ChunkKind Kind;
            public readonly string Text;

            internal Chunk(ChunkKind kind, string text) {
                Kind = kind;
                Text = text;
            }
        }

        public readonly IList<Chunk> Chunks;
        public readonly uint Priority;
        public readonly AvailabilityKind Availability;
        public readonly IList<string> Annotations;
        public readonly string Comment;

        internal unsafe CodeCompletion(Interop.CompletionResult* native) {
            IntPtr completionString = native->completionString;
            Chunks = new Chunk[Interop.clang_getNumCompletionChunks(completionString)];
            for (uint i = 0; i < Chunks.Count(); ++i) {
                Chunks[(int)i] = new Chunk(
                    Interop.clang_getCompletionChunkKind(completionString, i),
                    Interop.clang_getCompletionChunkText(completionString, i).ManagedString);
            }
            Priority = Interop.clang_getCompletionPriority(completionString);
            Availability = Interop.clang_getCompletionAvailability(completionString);
            Annotations = new string[Interop.clang_getCompletionNumAnnotations(completionString)];
            for (uint i = 0; i < Annotations.Count(); ++i) {
                Annotations[(int)i] = Interop.clang_getCompletionAnnotation(completionString, i).ManagedString;
            }
            Comment = Interop.clang_getCompletionAnnotation(completionString).ManagedString;
        }

        [Flags]
        public enum Options {
            None = 0x00,
            IncludeMacros = 0x01,
            IncludeCodePatterns = 0x02,
            IncludeBriefComments = 0x04
        }

        public enum AvailabilityKind {
            Available,
            Deprecated,
            NotAvailable,
            NotAccessible
        }

        public static Options DefaultOptions() {
            return (Options)Interop.clang_defaultCodeCompleteOptions();
        }
    }
}