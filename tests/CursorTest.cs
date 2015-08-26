using System.Collections.Generic;
using System.Linq;
using ClangSharp;
using NUnit.Framework;

namespace tests {
    [TestFixture]
    public class CursorTest : TestBase {
        [Test]
        public void TestVisitAll() {
            var cursors = new List<Cursor>();
            Main.Cursor.VisitChildren((cursor, parent) => {
                if (cursor.Location.IsValid) {
                    cursors.Add(cursor);
                }
                return Cursor.ChildVisitResult.Continue;
            });
            var myCursors = (
                from cursor in cursors
                where !cursor.ToString().Contains("unknown")
                where !cursor.ToString().Contains("Program Files")
                select cursor.ToString()).ToList();
            foreach (string symbol in new[] { "iostream", "fake-class.h", "OpaqueClass", "FakeClass", "main" }) {
                Assert.IsTrue(myCursors.Any(cursor => cursor.Contains(symbol)));
            }
        }

    }
}
