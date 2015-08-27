using System.IO;
using ClangSharp;
using NUnit.Framework;
using tests.Properties;

namespace tests {
    public class TestBase {
        protected static string FakeClassCpp = Path.Combine(Path.GetTempPath(), "fake-class.cpp");
        protected static string FakeClassH = Path.Combine(Path.GetTempPath(), "fake-class.h");
        protected static string OpaqueClassH = Path.Combine(Path.GetTempPath(), "opaque-class.h");
        protected static string MainCpp = Path.Combine(Path.GetTempPath(), "fake-main.cpp");
        protected static string KitchenSinkCpp = Path.Combine(Path.GetTempPath(), "kitchen-sink.cpp");

        protected static Index Index;
        protected static TranslationUnit Main;
        protected static TranslationUnit Class;
        protected static TranslationUnit KitchenSink;
    }
}
