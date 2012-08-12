using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ClangSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tests.Properties;

namespace tests {
    [TestClass]
    public class TestBase {
        protected static string FakeClassCpp = Path.Combine(Path.GetTempPath(), "fake-class.cpp");
        protected static string FakeClassH = Path.Combine(Path.GetTempPath(), "fake-class.h");
        protected static string OpaqueClassH = Path.Combine(Path.GetTempPath(), "opaque-class.h");
        protected static string MainCpp = Path.Combine(Path.GetTempPath(), "fake-main.cpp");

        protected static Index Index;
        protected static TranslationUnit Main;
        protected static TranslationUnit Class;

        [AssemblyInitialize]
        public static void CreateTestFiles(TestContext context) {
            System.IO.File.WriteAllText(FakeClassCpp, Resources.fake_class_cpp);
            System.IO.File.WriteAllText(FakeClassH, Resources.fake_class_h);
            System.IO.File.WriteAllText(OpaqueClassH, Resources.opaque_class_h);
            System.IO.File.WriteAllText(MainCpp, Resources.fake_main_cpp);
            var options = new[] { Options.Weverything };
            var unsavedFiles = new UnsavedFile[] { };
            Index = new Index();
            Main = Index.CreateTranslationUnit(MainCpp, options, unsavedFiles);
            Class = Index.CreateTranslationUnit(FakeClassCpp, options, unsavedFiles);
        }

        [AssemblyCleanup]
        public static void DeleteTestFiles() {
            new[] { FakeClassCpp, FakeClassH, OpaqueClassH, MainCpp }.ToList().ForEach(System.IO.File.Delete);
            Main.Dispose();
            Index.Dispose();
        }
    }
}
