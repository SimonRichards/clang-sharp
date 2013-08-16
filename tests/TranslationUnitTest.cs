using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests {
    [TestClass]
    public class TranslationUnitTest : TestBase {

        [TestMethod]
        public void TestSpelling() {
            Assert.IsTrue(Main.Spelling.Contains("fake-main.cpp"));
        }

        [TestMethod]
        public void TestInclusions() {
            var inclusions = new List<string>();
            Main.VisitInclusions((file, stack) => inclusions.Add(file.Name));
            Assert.IsTrue(inclusions.Any(inclusion => inclusion.Contains("fake-class.h")));
        }

        [TestMethod]
        public void TestDiagnostics() {
            Assert.IsTrue(Main.NumDiagnostics > 0);
            Assert.IsTrue(Class.NumDiagnostics == Class.Diagnostics.Count());
        }

        [TestMethod]
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
