using NUnit.Framework;
using System;
using System.IO;

namespace Vim.G3d.CppCLR.Tests
{
    public static class CppCLRTests
    {
        public static readonly string ProjectFolder = new DirectoryInfo(Properties.Resources.ProjDir.Trim()).FullName;
        public static string RootFolder = Path.Combine(ProjectFolder, "..", "..");
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
    }
}
