using System;
using System.Collections.Generic;
using System.Linq;

namespace ClangSharp {
    public static class Util {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> collection, T tail) {
            foreach (T val in collection) {
                yield return val;
            }
            yield return tail;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> collection, T head) {
            yield return head;
            foreach (T val in collection) {
                yield return val;
            }
        }

        public static string Substring(this string source, SourceRange range) {
            return source.Substring(range.Start.Offset, range.End.Offset - range.Start.Offset);
        }

        public static T[] Slice<T>(this IList<T> collection, SourceRange range) {
            return Slice(collection, range.Start.Offset, range.End.Offset - range.Start.Offset);
        }

        public static bool All(this IEnumerable<bool> collection) {
            foreach (bool item in collection) {
                if (!item) {
                    return false;
                }
            }
            return true;
        }

        public static T[] Slice<T>(this IList<T> collection, int start, int length) {
            var res = new T[length];
            for (int index = start, count = 0; count < length; ++count, ++index) {
                res[count] = collection[index];
            }
            return res;
        }

        static readonly int[][] _boms = new[] {
            new[] { 0xef, 0xbb, 0xbf },
            new[] { 0xfe, 0xff, 0x00, 0x00 },
            new[] { 0x00, 0x00, 0xfe, 0xff },
            new[] { 0xff, 0xfe }
        };

        public static int GetBomLength(this byte[] input) {
            foreach (var bom in _boms) {
                if (input.Length >= bom.Length && bom.Select((value, i) => input[i] == value).All()) {
                    return bom.Length;
                }
            }
            return 0;
        }

        public static IEnumerable<uint> Range(uint start, uint count) {
            for (uint i = start, j = 0; j < count; ++i, ++j) {
                yield return i;
            }
        }

        public static T MaxBy<T, C>(this IEnumerable<T> collection, Converter<T, C> converter) where C : IComparable {
            var enumerator = collection.GetEnumerator();
            if (enumerator.MoveNext()) {
                T best = enumerator.Current;
                C bestScore = converter(best);
                while (enumerator.MoveNext()) {
                    C score = converter(enumerator.Current);
                    if (score.CompareTo(bestScore) > 0) {
                        best = enumerator.Current;
                    }
                }
                return best;
            } else {
                throw new ArgumentException("Cannot search empty collection");
            }
        }

        public static T MinBy<T, C>(this IEnumerable<T> collection, Converter<T, C> converter) where C : IComparable {
            var enumerator = collection.GetEnumerator();
            if (enumerator.MoveNext()) {
                T best = enumerator.Current;
                C bestScore = converter(best);
                while (enumerator.MoveNext()) {
                    C score = converter(enumerator.Current);
                    if (score.CompareTo(bestScore) < 0) {
                        best = enumerator.Current;
                    }
                }
                return best;
            } else {
                throw new ArgumentException("Cannot search empty collection");
            }
        }
    }
}
