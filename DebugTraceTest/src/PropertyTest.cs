using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static DebugTrace.CSharp;
using DebugTrace;

namespace DebugTraceTest {
    [TestClass]
    public class PropertyTest {
        // testProperties
        private static readonly IDictionary<string, string> testProperties = new Dictionary<string, string>() {
            {nameof(TraceBase.EnterString              ), @"_Enter_ {0}.{1} ({2}:{3:D})"},
            {nameof(TraceBase.LeaveString              ), @"_Leave_ {0}.{1} ({2}:{3:D})"},
            {nameof(TraceBase.ThreadBoundaryString     ), @"_Thread_ {0}"},
            {nameof(TraceBase.ClassBoundaryString      ), @"_ {0} _"},
            {nameof(TraceBase.CodeIndentString         ), @"||"},
            {nameof(TraceBase.DataIndentString         ), @"``"},
            {nameof(TraceBase.LimitString              ), @"<Limit>"},
            {nameof(TraceBase.DefaultNameSpaceString   ), @"<DefaultNameSpace>"},
            {nameof(TraceBase.NonPrintString           ), @"<NonPrint>"},
            {nameof(TraceBase.CyclicReferenceString    ), @"<CyclicReference>"},
            {nameof(TraceBase.VarNameValueSeparator    ), @"\s<=\s"},
            {nameof(TraceBase.KeyValueSeparator        ), @"\s::\s"},
            {nameof(TraceBase.PrintSuffixFormat        ), @"\s[{2}:{3:D}]"},
            {nameof(TraceBase.DateTimeFormat           ), @"{0:MM-dd-yyyy hh:mm:ss.fffffffK}"},
            {nameof(TraceBase.LogDateTimeFormat        ), @"{0:MM-dd-yyyy hh:mm:ss.fff} [{1:D2}] {2}"}, // since 1.3.0
            {nameof(TraceBase.MaxDataOutputWidth       ), "40"},
            {nameof(TraceBase.CollectionLimit          ), "8"},
            {nameof(TraceBase.StringLimit              ), "32"},
            {nameof(TraceBase.ReflectionNestLimit      ), "2"},
            {nameof(TraceBase.NonPrintProperties       ), "DebugTraceTest.Point.X, DebugTraceTest.Point.Y"},
            {nameof(TraceBase.DefaultNameSpace         ), "DebugTraceTest"},
            {nameof(TraceBase.ReflectionClasses        ), "DebugTraceTest.Point3,System.DateTime"},
            {nameof(TraceBase.OutputNonPublicFields    ), "true"},  // since 1.4.4
            {nameof(TraceBase.OutputNonPublicProperties), "True"},  // since 1.4.4
        };

        // emptyProperties
        private static readonly IDictionary<string, string> emptyProperties = new Dictionary<string, string>();

        // ClassInit
        [ClassInitialize]
        public static void ClassInit(TestContext context) {
            WriteToProperties(testProperties);
            TraceBase.InitClass();
        }

        // ClassCleanup
        [ClassCleanup]
        public static void ClassCleanup() {
            WriteToProperties(emptyProperties);
            TraceBase.InitClass();
        }

        // WriteToProperties
        private static void WriteToProperties(IDictionary<string, string> values) {
            var resourceFileInfo = TraceBase.Resource.FileInfo;
            if (resourceFileInfo.Exists)
                resourceFileInfo.Delete();

            using (FileStream stream = resourceFileInfo.Open(FileMode.Create, FileAccess.Write, FileShare.None)) {
                var encoding = new UTF8Encoding();

                foreach (var key in values.Keys) {
                    var line = $"{key} = {values[key]}\n";
                    var bytes = encoding.GetBytes(line);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }

        // TraceBase.InitClass
        [TestMethod]
        public void TraceBaseInitClass() {
            Assert.AreEqual(TraceBase.EnterString            , Resource.Unescape(testProperties[nameof(TraceBase.EnterString            )]));
            Assert.AreEqual(TraceBase.LeaveString            , Resource.Unescape(testProperties[nameof(TraceBase.LeaveString            )]));
            Assert.AreEqual(TraceBase.ThreadBoundaryString   , Resource.Unescape(testProperties[nameof(TraceBase.ThreadBoundaryString   )]));
            Assert.AreEqual(TraceBase.ClassBoundaryString    , Resource.Unescape(testProperties[nameof(TraceBase.ClassBoundaryString    )]));
            Assert.AreEqual(TraceBase.CodeIndentString       , Resource.Unescape(testProperties[nameof(TraceBase.CodeIndentString       )]));
            Assert.AreEqual(TraceBase.DataIndentString       , Resource.Unescape(testProperties[nameof(TraceBase.DataIndentString       )]));
            Assert.AreEqual(TraceBase.LimitString            , Resource.Unescape(testProperties[nameof(TraceBase.LimitString            )]));
            Assert.AreEqual(TraceBase.DefaultNameSpaceString , Resource.Unescape(testProperties[nameof(TraceBase.DefaultNameSpaceString )]));
            Assert.AreEqual(TraceBase.NonPrintString         , Resource.Unescape(testProperties[nameof(TraceBase.NonPrintString         )]));
            Assert.AreEqual(TraceBase.CyclicReferenceString  , Resource.Unescape(testProperties[nameof(TraceBase.CyclicReferenceString  )]));
            Assert.AreEqual(TraceBase.VarNameValueSeparator  , Resource.Unescape(testProperties[nameof(TraceBase.VarNameValueSeparator  )]));
            Assert.AreEqual(TraceBase.KeyValueSeparator      , Resource.Unescape(testProperties[nameof(TraceBase.KeyValueSeparator      )]));
            Assert.AreEqual(TraceBase.PrintSuffixFormat      , Resource.Unescape(testProperties[nameof(TraceBase.PrintSuffixFormat      )]));
            Assert.AreEqual(TraceBase.DateTimeFormat         , Resource.Unescape(testProperties[nameof(TraceBase.DateTimeFormat         )]));
            Assert.AreEqual(TraceBase.LogDateTimeFormat      , Resource.Unescape(testProperties[nameof(TraceBase.LogDateTimeFormat      )])); // since 1.3.0
            Assert.AreEqual(TraceBase.MaxDataOutputWidth .ToString(),            testProperties[nameof(TraceBase.MaxDataOutputWidth     )]);
            Assert.AreEqual(TraceBase.CollectionLimit    .ToString(),            testProperties[nameof(TraceBase.CollectionLimit        )]);
            Assert.AreEqual(TraceBase.StringLimit        .ToString(),            testProperties[nameof(TraceBase.StringLimit            )]);
            Assert.AreEqual(TraceBase.ReflectionNestLimit.ToString(),            testProperties[nameof(TraceBase.ReflectionNestLimit    )]);
        // 1.4.1
        //  AssertAreEqual (TraceBase.NonPrintProperties     ,                   testProperties[nameof(TraceBase.NonPrintProperties     )].Split(',').Select(s => s.Trim()).ToArray());
            Assert.AreEqual(TraceBase.DefaultNameSpace       ,                   testProperties[nameof(TraceBase.DefaultNameSpace       )]);

            var reflectionClasses = new HashSet<string>(testProperties[nameof(TraceBase.ReflectionClasses)].Split(',').Select(s => s.Trim()));
            reflectionClasses.Add(typeof(Tuple).FullName); // Tuple
            reflectionClasses.Add(typeof(ValueTuple).FullName); // ValueTuple
            AssertAreEqual(TraceBase.ReflectionClasses, reflectionClasses);

            Assert.AreEqual(TraceBase.OutputNonPublicFields    , bool.Parse(testProperties[nameof(TraceBase.OutputNonPublicFields    )])); // since 1.4.4
            Assert.AreEqual(TraceBase.OutputNonPublicProperties, bool.Parse(testProperties[nameof(TraceBase.OutputNonPublicProperties)])); // since 1.4.4
        }

        // AssertAreEqual
        private static void AssertAreEqual<T>(IEnumerable<T> enumerable1, IEnumerable<T> enumerable2) {
            if (enumerable1 == null) {
                if (enumerable2 == null) return;
                Assert.Fail($"enumerable1 == null, enumerable2 != null");
            }
            if (enumerable2 == null)
                Assert.Fail($"enumerable1 != null, enumerable2 == null");
            if (enumerable1.Count() != enumerable2.Count())
                Assert.Fail($"enumerable1.Count() = {enumerable1.Count()}, enumerable2.Count() = {enumerable2.Count()}");
            for (var index = 0; index < enumerable1.Count(); ++index)
                if (!enumerable1.ElementAt(index).Equals(enumerable1.ElementAt(index)))
                    Assert.Fail($"enumerable1.ElementAt({index}) = {enumerable1.ElementAt(index)}, enumerable2.ElementAt({index}) = {enumerable1.ElementAt(index)}");
        }

        // EnterString
        [TestMethod]
        public void EnterString() {
            Trace.Enter();
            StringAssert.Contains(Trace.LastLog, "_Enter_");
        }

        // LeaveString
        [TestMethod]
        public void LeaveString() {
            Trace.Leave();
            StringAssert.Contains(Trace.LastLog, "_Leave_");
        }

        // CodeIndentString
        [TestMethod]
        public void CodeIndentString() {
            Trace.Enter();
            Trace.Enter();
            StringAssert.Contains(Trace.LastLog, TraceBase.CodeIndentString);
            Trace.Leave();
            Trace.Leave();
        }

        // DataIndentString
        [TestMethod]
        public void DataIndentString() {
            Trace.Print("contact", new Contact(1, "Akane", "Apple", new DateTime(2018, 4, 1)));
            StringAssert.Contains(Trace.LastLog, TraceBase.DataIndentString);
        }

        // LimitString / CollectionLimit
        [TestMethod]
        public void LimitString() {
            Trace.Print("value", new int[TraceBase.CollectionLimit]);
            StringAssert.Contains(Trace.LastLog, ", 0}");

            Trace.Print("value", new int[TraceBase.CollectionLimit + 1]);
            StringAssert.Contains(Trace.LastLog, ", 0, " + TraceBase.LimitString + "}");
        }

        // DefaultNameSpaceString / DefaultNameSpace
        [TestMethod]
        public void DefaultNameSpaceString() {
            Trace.Print("point", new Point(1, 2));
            StringAssert.Contains(Trace.LastLog, TraceBase.DefaultNameSpaceString + ".Point");
        }

        // NonPrintString
        [TestMethod]
        public void NonPrintString() {
            Trace.Print("point", new Point(1, 2));
            StringAssert.Contains(Trace.LastLog, TraceBase.NonPrintString);
        }

        // CyclicReferenceString
        [TestMethod]
        public void CyclicReferenceString() {
            var node1 = new Node<int>(1);
            var node2 = new Node<int>(2, node1, node1);
            node1.Left = node2;
            node1.Right = node2;
            Trace.Print("node1", node1);
            StringAssert.Contains(Trace.LastLog, TraceBase.CyclicReferenceString);
        }

        // VarNameValueSeparator
        [TestMethod]
        public void VarNameValueSeparator() {
            var value = 1;
            Trace.Print("value", value);
            StringAssert.Contains(Trace.LastLog, "value" + TraceBase.VarNameValueSeparator + value);
        }

        // KeyValueSeparator
        [TestMethod]
        public void KeyValueSeparator() {
            Trace.Print("value", new Dictionary<int, int>() {{1, 2}});
            StringAssert.Contains(Trace.LastLog, "1" + TraceBase.KeyValueSeparator + "2");
        }

        // ReflectionClasses
        [TestMethod]
        public void ReflectionClasses() {
            var rectangle = new Rectangle(1, 2, 3, 4);
            Trace.Print("rectangle", rectangle); // use ToString method
            StringAssert.Contains(Trace.LastLog, rectangle.ToString());

            var point3 = new Point3(1, 2, 3);
            Trace.Print("point3", point3); // use reflection
            StringAssert.Contains(Trace.LastLog, "X" + TraceBase.KeyValueSeparator + point3.X);

            var dateTime = DateTime.Now;
            Trace.Print("dateTime", dateTime); // use reflection
            StringAssert.Contains(Trace.LastLog, TraceBase.KeyValueSeparator);
        }

        public class Inner {
            private   int PrivateField      = 1;
            protected int ProtectedField    = 2;
            public    int PublicField       = 3;
            private   int PrivateProperty   {get;} = 4;
            protected int ProtectedProperty {get;} = 5;
            public    int PublicProperty    {get;} = 6;
        }

        // OutputNonPublicFields / since 1.4.4
        [TestMethod]
        public void OutputNonPublicFields_Properties() {
            TraceBase.OutputNonPublicFields     = false;
            TraceBase.OutputNonPublicProperties = false;
            Trace.Print("value", new Inner());
            Assert.IsFalse(Trace.LastLog.Contains("PrivateField"     ));
            Assert.IsFalse(Trace.LastLog.Contains("ProtectedField"   ));
            Assert.IsTrue (Trace.LastLog.Contains("PublicField"      ));
            Assert.IsFalse(Trace.LastLog.Contains("PrivateProperty"  ));
            Assert.IsFalse(Trace.LastLog.Contains("ProtectedProperty"));
            Assert.IsTrue (Trace.LastLog.Contains("PublicProperty"   ));

            TraceBase.OutputNonPublicFields     = true;
            TraceBase.OutputNonPublicProperties = false;
            Trace.Print("value", new Inner());
            Assert.IsTrue (Trace.LastLog.Contains("PrivateField"     ));
            Assert.IsTrue (Trace.LastLog.Contains("ProtectedField"   ));
            Assert.IsTrue (Trace.LastLog.Contains("PublicField"      ));
            Assert.IsFalse(Trace.LastLog.Contains("PrivateProperty"  ));
            Assert.IsFalse(Trace.LastLog.Contains("ProtectedProperty"));
            Assert.IsTrue (Trace.LastLog.Contains("PublicProperty"   ));

            TraceBase.OutputNonPublicFields     = false;
            TraceBase.OutputNonPublicProperties = true;
            Trace.Print("value", new Inner());
            Assert.IsFalse(Trace.LastLog.Contains("PrivateField"     ));
            Assert.IsFalse(Trace.LastLog.Contains("ProtectedField"   ));
            Assert.IsTrue (Trace.LastLog.Contains("PublicField"      ));
            Assert.IsTrue (Trace.LastLog.Contains("PrivateProperty"  ));
            Assert.IsTrue (Trace.LastLog.Contains("ProtectedProperty"));
            Assert.IsTrue (Trace.LastLog.Contains("PublicProperty"   ));

            TraceBase.OutputNonPublicFields     = true;
            TraceBase.OutputNonPublicProperties = true;
            Trace.Print("value", new Inner());
            Assert.IsTrue (Trace.LastLog.Contains("PrivateField"     ));
            Assert.IsTrue (Trace.LastLog.Contains("ProtectedField"   ));
            Assert.IsTrue (Trace.LastLog.Contains("PublicField"      ));
            Assert.IsTrue (Trace.LastLog.Contains("PrivateProperty"  ));
            Assert.IsTrue (Trace.LastLog.Contains("ProtectedProperty"));
            Assert.IsTrue (Trace.LastLog.Contains("PublicProperty"   ));
        }
    }
}
