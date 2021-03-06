﻿using NUnit.Framework;
using System;
using System.IO;

namespace Vim.G3d.CppCLR.Tests
{
    public static class CppCLRTests
    {
        public static string BaseInputDataPath => Path.Combine(TestContext.CurrentContext.TestDirectory,
            "..", "..", "..", "..", "data"); // 4, not 5, 4

        public static string InputDataPath => Path.Combine(BaseInputDataPath, "assimp", "test");

        public static string TestOutputFolder => Path.Combine(InputDataPath, "..", "..", "g3d");

        public static string[] GeneratedG3ds => Directory.GetFiles(TestOutputFolder, "*.g3d");

        public static string TestOutputFolderCpp => Path.Combine(TestOutputFolder, "cpp");

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

        public static void Main(string[] args)
        {
            CppTest();
        }
    }
}
