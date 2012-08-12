using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClangSharp;

namespace tests {
    [TestClass]
    public class CursorTest : TestBase {
        [TestMethod]
        public void TestVisitAll() {
            var cursors = new List<Cursor>();
            Main.Cursor.VisitChildren((cursor, parent) => {
                cursors.Add(cursor);
                return Cursor.ChildVisitResult.Continue;
            });
            var myCursors = (
                from cursor in cursors
                where !cursor.ToString().Contains("unknown")
                where !cursor.ToString().Contains("Program Files")
                select cursor.ToString()).ToList();
            Assert.IsTrue(myCursors.Any(cursor => cursor.Contains("fake-main.cpp")));
            Assert.IsTrue(myCursors.Any(cursor => cursor.Contains("fake-class.h")));
        }
    }
}
