using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Vim.G3d.Tests.Utils;

namespace Vim.G3d.CppCLR.Tests
{
    public static class CppCLRTests
    {
        public static string BaseInputDataPath => Path.Combine(TestContext.CurrentContext.TestDirectory,
            "..", "..", "..", "..", "data"); // 4, not 5, 4

        public static string InputDataPath => Path.Combine(BaseInputDataPath, "assimp", "test");

        public static string TestOutputFolder => Path.Combine(InputDataPath, "..", "..", "g3d");

        public static string[] GeneratedG3ds => Directory.GetFiles(TestOutputFolder, "*.g3d");


        [Test]
        public static void CppTest()
        {
            var x = new ManagedG3d();

            foreach (var f in GeneratedG3ds)
            {
                Console.WriteLine("File name = " + f);
                x.Load(f);
            }
        }

        public static void Main(string[] args)
        {
            CppTest();
        }
    }
}
