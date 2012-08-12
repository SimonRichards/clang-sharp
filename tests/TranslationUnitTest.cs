using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClangSharp;
using tests.Properties;
using System.IO;

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
            Assert.IsTrue(Main.Diagnostics.Any(diag => diag.Format().Contains("newline")));
            Assert.IsTrue(Class.NumDiagnostics == Class.Diagnostics.Count());
            Assert.IsTrue(Class.Diagnostics.First().NumFixits == Class.Diagnostics.First().Fixits.Count());
        }
    }
}
