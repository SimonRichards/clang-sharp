using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace tests {
    [TestFixture]
    public class TranslationUnitTest : TestBase {

        [Test]
        public void TestSpelling() {
            Assert.IsTrue(Main.Spelling.Contains("fake-main.cpp"));
        }

        [Test]
        public void TestInclusions() {
            var inclusions = new List<string>();
            Main.VisitInclusions((file, stack) => inclusions.Add(file.Name));
            Assert.IsTrue(inclusions.Any(inclusion => inclusion.Contains("fake-class.h")));
        }

        [Test]
        public void TestDiagnostics() {
            Assert.IsTrue(Main.NumDiagnostics > 0);
            Assert.IsTrue(Class.NumDiagnostics == Class.Diagnostics.Count());
        }

        [Test]
        public void TestComments() {
            string source = System.IO.File.ReadAllText(KitchenSinkCpp);
            var comments = KitchenSink.Comments;
            Assert.AreEqual(7, comments.Count());
            foreach (var comment in comments) {
                int start = comment.Extent.Start.Offset, end = comment.Extent.End.Offset;
                Assert.AreEqual(comment.Spelling, source.Substring(start, end - start));
            }
        }
    }
}
