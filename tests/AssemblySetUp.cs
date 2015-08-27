using System;
using NUnit.Framework;
using ClangSharp;
using tests.Properties;

namespace tests {
	[SetUpFixture]
	public class AssemblySetUp : TestBase {
		[SetUp]
		public static void CreateTestFiles() {
			System.IO.File.WriteAllText(FakeClassCpp, Resources.fake_class_cpp);
			System.IO.File.WriteAllText(FakeClassH, Resources.fake_class_h);
			System.IO.File.WriteAllText(OpaqueClassH, Resources.opaque_class_h);
			System.IO.File.WriteAllText(MainCpp, Resources.fake_main_cpp);
			System.IO.File.WriteAllText(KitchenSinkCpp, Resources.kitchen_sink);
			var args = new[] { Options.Weverything };
			var unsavedFiles = new UnsavedFile[] { };
			var options = TranslationUnitFlags.IncludeBriefCommentsInCodeCompletion | TranslationUnitFlags.DetailedPreprocessingRecord;
			Index = new Index();
			Main = Index.CreateTranslationUnit(MainCpp, args, unsavedFiles, options);
			Class = Index.CreateTranslationUnit(FakeClassCpp, args, unsavedFiles, options);
			KitchenSink = Index.CreateTranslationUnit(KitchenSinkCpp, args, unsavedFiles, options);
		}

		[TearDown]
		public static void DeleteTestFiles() {
			foreach (string file in new[] { FakeClassCpp, FakeClassH, OpaqueClassH, MainCpp, KitchenSinkCpp }) {
				System.IO.File.Delete(file);
			}
			Main.Dispose();
			Index.Dispose();
		}
	}
}

