using NUnit.Framework;
using System;
using System.IO;

namespace Vim.G3d.CppCLR.Tests
{
    public static class CppCLRTests
    {
        public static readonly string ProjectFolder = new DirectoryInfo(Properties.Resources.ProjDir.Trim()).FullName;
        public static string RootFolder = Path.Combine(ProjectFolder, "..", "..");
        public static string TestModelFolder = Path.Combine(RootFolder, "test-data", "models");
        public static string TestOutputFolder = Path.Combine(RootFolder, "test-data", "output");
        public static string TestOutputFolderCpp => Path.Combine(TestOutputFolder, "cpp");
        public static string[] GeneratedG3ds => Directory.GetFiles(TestOutputFolder, "*.g3d");

        public static void OutputStats(ManagedG3d x)
        {
            Console.WriteLine($"{x.Count()} attributes");
            for (var i = 0; i < x.Count(); ++i)
                Console.WriteLine($"{i} attribute {x.AttributeName(i)} has {x.AttributeElementCount(i)} elements");
        }

        [Test]
        public static void CppTest()
        {
            var x = new ManagedG3d();

            Directory.CreateDirectory(TestOutputFolderCpp);

            foreach (var f in GeneratedG3ds)
            {
                Console.WriteLine($"Loading file {f}");
                x.Load(f);
                var output = Path.Combine(TestOutputFolderCpp, Path.GetFileName(f));
                OutputStats(x);
                x.Write(output);
                x.Load(output);
                OutputStats(x);
            }
        }

        [Test]
        public static void UnexpectedCppTest()
        {
            // This test validates that unrecognized g3d attributes are simply ignored in the deserialization process.
            //
            // "unexpected.g3d" was generated with the following attributes:
            // - "g3d:instance:potato:0:int32:1" - an int32 array containing a single element (5)
            // - "g3d:instance:beep:0:UNKNOWN:1" - where the "UNKNOWN" data type was temporarily implemented as a ulong (8 bits), and containing a single element (42)

            var managedG3d = new ManagedG3d();

            managedG3d.Load(Path.Combine(TestModelFolder, "unexpected.g3d"));

            Assert.AreEqual(2, managedG3d.Count());
            Assert.AreEqual("g3d:corner:index:0:int32:1", managedG3d.AttributeName(0)); // Automatically generated.
            Assert.AreEqual("g3d:instance:potato:0:int32:1", managedG3d.AttributeName(1)); // Added.
            // The attribute "g3d:instance:beep:0:UNKNOWN:1" should be ignored because the datatype "UNKNOWN" is not recognized.
        }
    }
}
